#!/usr/bin/env python3
from __future__ import annotations

import json
import re
import urllib.request
from dataclasses import dataclass
from datetime import date, datetime, time, timedelta, timezone
from pathlib import Path
from typing import Iterable
from zoneinfo import ZoneInfo

from dateutil.rrule import rrulestr

ROOT = Path(__file__).resolve().parents[1]
DOCS = ROOT / "docs"
ICS_URL = (
    "https://calendar.google.com/calendar/ical/"
    "ef1b943ddbe088897fb86e9ddac3a57f3bbc0f303c93f6608369ff1926fc97cc%40group.calendar.google.com"
    "/public/basic.ics"
)
LOCAL_TZ = ZoneInfo("Europe/Sofia")


@dataclass
class Event:
    uid: str
    summary: str
    start: datetime
    end: datetime
    location: str
    status: str
    recurrence_id: datetime | None = None
    rrule: str | None = None
    exdates: list[datetime] | None = None
    all_day: bool = False


def unfold_ics(text: str) -> list[str]:
    lines: list[str] = []
    for raw in text.splitlines():
        if raw.startswith((" ", "\t")) and lines:
            lines[-1] += raw[1:]
        else:
            lines.append(raw)
    return lines


def parse_prop(line: str) -> tuple[str, dict[str, str], str]:
    left, value = line.split(":", 1)
    parts = left.split(";")
    name = parts[0]
    params: dict[str, str] = {}
    for param in parts[1:]:
        if "=" in param:
            k, v = param.split("=", 1)
            params[k.upper()] = v
    return name.upper(), params, value


def parse_dt(value: str, params: dict[str, str]) -> tuple[datetime, bool]:
    if params.get("VALUE") == "DATE":
        dt = datetime.combine(date.fromisoformat(value), time.min, tzinfo=LOCAL_TZ)
        return dt, True

    tz = LOCAL_TZ
    if value.endswith("Z"):
        tz = timezone.utc
        value = value[:-1]
    elif "TZID" in params:
        tz = ZoneInfo(params["TZID"])

    fmt = "%Y%m%dT%H%M%S" if len(value) == 15 else "%Y%m%dT%H%M"
    dt = datetime.strptime(value, fmt).replace(tzinfo=tz)
    return dt, False


def utc_key(dt: datetime) -> str:
    return dt.astimezone(timezone.utc).isoformat()


def load_events() -> list[Event]:
    text = urllib.request.urlopen(ICS_URL, timeout=30).read().decode("utf-8", "ignore")
    lines = unfold_ics(text)
    blocks: list[list[str]] = []
    current: list[str] | None = None
    for line in lines:
        if line == "BEGIN:VEVENT":
            current = []
        elif line == "END:VEVENT":
            if current is not None:
                blocks.append(current)
                current = None
        elif current is not None:
            current.append(line)

    events: list[Event] = []
    for block in blocks:
        values: dict[str, list[tuple[dict[str, str], str]]] = {}
        for line in block:
            name, params, value = parse_prop(line)
            values.setdefault(name, []).append((params, value))

        if not values.get("UID") or not values.get("DTSTART"):
            continue

        start, all_day = parse_dt(*values["DTSTART"][0][::-1])
        if values.get("DTEND"):
            end, _ = parse_dt(*values["DTEND"][0][::-1])
        else:
            end = start + (timedelta(days=1) if all_day else timedelta(hours=1))

        recurrence_id = None
        if values.get("RECURRENCE-ID"):
            recurrence_id, _ = parse_dt(*values["RECURRENCE-ID"][0][::-1])

        exdates: list[datetime] = []
        for params, value in values.get("EXDATE", []):
            for piece in value.split(","):
                exdate, _ = parse_dt(piece, params)
                exdates.append(exdate)

        events.append(
            Event(
                uid=values["UID"][0][1],
                summary=values.get("SUMMARY", [({}, "Untitled event")])[0][1],
                start=start,
                end=end,
                location=values.get("LOCATION", [({}, "")])[0][1].replace("\\,", ","),
                status=values.get("STATUS", [({}, "CONFIRMED")])[0][1],
                recurrence_id=recurrence_id,
                rrule=values.get("RRULE", [({}, "")])[0][1] or None,
                exdates=exdates,
                all_day=all_day,
            )
        )
    return events


def build_upcoming(events: Iterable[Event], limit: int = 6) -> list[dict[str, object]]:
    now = datetime.now(LOCAL_TZ)
    horizon = now + timedelta(days=400)
    overrides: dict[str, dict[str, Event]] = {}
    bases: list[Event] = []

    for event in events:
        if event.status.upper() == "CANCELLED":
            continue
        if event.recurrence_id is not None:
            overrides.setdefault(event.uid, {})[utc_key(event.recurrence_id)] = event
        else:
            bases.append(event)

    upcoming: list[Event] = []
    for event in bases:
        if event.rrule:
            rule = rrulestr(event.rrule, dtstart=event.start)
            exdates = {utc_key(dt) for dt in (event.exdates or [])}
            duration = event.end - event.start
            cursor = now - timedelta(seconds=1)
            seen = 0
            while seen < 64:
                occurrence = rule.after(cursor, inc=True)
                if occurrence is None or occurrence > horizon:
                    break
                seen += 1
                cursor = occurrence + timedelta(seconds=1)
                occurrence_key = utc_key(occurrence)
                if occurrence_key in exdates:
                    continue
                if occurrence_key in overrides.get(event.uid, {}):
                    instance = overrides[event.uid][occurrence_key]
                else:
                    instance = Event(
                        uid=event.uid,
                        summary=event.summary,
                        start=occurrence,
                        end=occurrence + duration,
                        location=event.location,
                        status=event.status,
                        all_day=event.all_day,
                    )
                if instance.end >= now:
                    upcoming.append(instance)
        else:
            if event.end >= now:
                upcoming.append(event)

    upcoming.sort(key=lambda e: e.start)
    trimmed = upcoming[:limit]
    return [
        {
            "title": event.summary,
            "start": event.start.astimezone(LOCAL_TZ).isoformat(),
            "end": event.end.astimezone(LOCAL_TZ).isoformat(),
            "location": event.location,
            "allDay": event.all_day,
        }
        for event in trimmed
    ]


SECTION_TEMPLATE = """<section class=\"bc-section bc-section--calendar\" id=\"calendar\">\n    <div class=\"container\">\n        <h2 class=\"bc-section-title\"><i class=\"bi bi-calendar3 me-2\"></i>{title}</h2>\n        <p class=\"bc-calendar-note\">{note}</p>\n        <div class=\"row g-3 mt-1\" data-events-feed data-lang=\"{lang}\" data-empty-label=\"{empty_label}\" data-error-label=\"{error_label}\" data-all-day-label=\"{all_day_label}\">\n            <div class=\"col-12\">\n                <div class=\"bc-event-empty\">\n                    <i class=\"bi bi-hourglass-split bc-event-empty-icon\"></i>\n                    <p>{loading_label}</p>\n                </div>\n            </div>\n        </div>\n        <div class=\"mt-4\">\n            <a href=\"https://calendar.google.com/calendar/u/0?cid=ZWYxYjk0M2RkYmUwODg4OTdmYjg2ZTlkZGFjM2E1N2YzYmJjMGYzMDNjOTNmNjYwODM2OWZmMTkyNmZjOTdjY0Bncm91cC5jYWxlbmRhci5nb29nbGUuY29t\" target=\"_blank\" rel=\"noopener noreferrer\" class=\"bc-calendar-link\"><i class=\"bi bi-calendar3 me-1\"></i>{view_label}<i class=\"bi bi-arrow-right ms-1\"></i></a>\n        </div>\n    </div>\n</section>"""


def patch_html(path: Path, *, lang: str, title: str, note: str, loading_label: str, empty_label: str, error_label: str, all_day_label: str, view_label: str) -> None:
    content = path.read_text()
    replacement = SECTION_TEMPLATE.format(
        title=title,
        note=note,
        lang=lang,
        loading_label=loading_label,
        empty_label=empty_label,
        error_label=error_label,
        all_day_label=all_day_label,
        view_label=view_label,
    )
    content, count = re.subn(
        r'<section class="bc-section bc-section--calendar" id="calendar">.*?</section>',
        replacement,
        content,
        count=1,
        flags=re.S,
    )
    if count != 1:
        raise RuntimeError(f"Could not patch calendar section in {path}")

    script_tag = '<script src="./events-feed.js"></script>'
    if script_tag not in content:
        content = content.replace(
            '<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>',
            '<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>\n' + script_tag,
            1,
        )
    path.write_text(content)


def main() -> None:
    upcoming = build_upcoming(load_events())
    (DOCS / 'events.json').write_text(json.dumps({"generatedAt": datetime.now(timezone.utc).isoformat(), "events": upcoming}, ensure_ascii=False, indent=2) + "\n")

    patch_html(
        DOCS / 'index.html',
        lang='bg',
        title='Предстоящи събития',
        note='Показваме следващите 6 събития от публичния календар на клуба.',
        loading_label='Зареждаме следващите събития…',
        empty_label='Няма предстоящи събития.',
        error_label='Не успяхме да заредим събитията точно сега.',
        all_day_label='Цял ден',
        view_label='Виж пълния календар',
    )
    patch_html(
        DOCS / 'en.html',
        lang='en',
        title='Upcoming Events',
        note='Showing the next 6 events from the club’s public calendar feed.',
        loading_label='Loading upcoming events…',
        empty_label='No upcoming events scheduled.',
        error_label='Could not load events right now.',
        all_day_label='All day',
        view_label='View full calendar',
    )


if __name__ == '__main__':
    main()
