(async () => {
  const containers = document.querySelectorAll('[data-events-feed]');
  if (!containers.length) return;

  const formatters = {
    bg: {
      month: new Intl.DateTimeFormat('bg-BG', { month: 'short', timeZone: 'Europe/Sofia' }),
      meta: new Intl.DateTimeFormat('bg-BG', { weekday: 'short', day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit', timeZone: 'Europe/Sofia' }),
      allDay: new Intl.DateTimeFormat('bg-BG', { weekday: 'short', day: '2-digit', month: 'short', timeZone: 'Europe/Sofia' }),
    },
    en: {
      month: new Intl.DateTimeFormat('en-US', { month: 'short', timeZone: 'Europe/Sofia' }),
      meta: new Intl.DateTimeFormat('en-US', { weekday: 'short', day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit', timeZone: 'Europe/Sofia' }),
      allDay: new Intl.DateTimeFormat('en-US', { weekday: 'short', day: '2-digit', month: 'short', timeZone: 'Europe/Sofia' }),
    }
  };

  function renderEmpty(container, icon, text) {
    container.innerHTML = `
      <div class="col-12">
        <div class="bc-event-empty">
          <i class="bi ${icon} bc-event-empty-icon"></i>
          <p>${text}</p>
        </div>
      </div>`;
  }

  function escapeHtml(value) {
    return String(value)
      .replaceAll('&', '&amp;')
      .replaceAll('<', '&lt;')
      .replaceAll('>', '&gt;')
      .replaceAll('"', '&quot;')
      .replaceAll("'", '&#39;');
  }

  function renderEvents(container, events) {
    const lang = container.dataset.lang === 'bg' ? 'bg' : 'en';
    const labels = formatters[lang];
    const allDayLabel = container.dataset.allDayLabel || 'All day';

    container.innerHTML = events.map((event) => {
      const start = new Date(event.start);
      const month = labels.month.format(start).replace('.', '').toUpperCase();
      const day = String(start.getDate());
      const meta = escapeHtml(event.allDay ? `${allDayLabel} · ${labels.allDay.format(start)}` : labels.meta.format(start).replace(',', ' ·'));
      const location = event.location ? `
        <div class="bc-event-location">
          <i class="bi bi-geo-alt me-1"></i>${escapeHtml(event.location)}
        </div>` : '';

      return `
        <div class="col-md-6 col-lg-4">
          <div class="bc-event-card">
            <div class="bc-event-date">
              <span class="bc-event-month">${escapeHtml(month)}</span>
              <span class="bc-event-day">${escapeHtml(day)}</span>
            </div>
            <div class="bc-event-body">
              <div class="bc-event-title">${escapeHtml(event.title)}</div>
              <div class="bc-event-meta">
                <i class="bi ${event.allDay ? 'bi-calendar' : 'bi-clock'}"></i>
                <span>${meta}</span>
              </div>
              ${location}
            </div>
          </div>
        </div>`;
    }).join('');
  }

  try {
    const response = await fetch('./events.json', { cache: 'no-store' });
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    const payload = await response.json();
    const events = Array.isArray(payload.events) ? payload.events.slice(0, 6) : [];

    containers.forEach((container) => {
      if (!events.length) {
        renderEmpty(container, 'bi-calendar-x', container.dataset.emptyLabel || 'No upcoming events.');
        return;
      }
      renderEvents(container, events);
    });
  } catch (error) {
    containers.forEach((container) => {
      renderEmpty(container, 'bi-exclamation-circle', container.dataset.errorLabel || 'Could not load events.');
    });
    console.error('Failed to load events feed', error);
  }
})();
