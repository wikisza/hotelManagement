const darkPalette = [
    "#4B0082", // Indigo
    "#2F4F4F", // Dark Slate Gray
    "#8B0000", // Dark Red
    "#556B2F", // Dark Olive Green
    "#483D8B", // Dark Slate Blue
    "#191970", // Midnight Blue
    "#2C2C54", // Deep Dark Blue
    "#3C3C3C"  // Dark Gray
];

document.addEventListener('DOMContentLoaded', function () {
    // CALENDAR - tylko jeśli element istnieje
    var calendarEl = document.getElementById('calendar');
    
    if (calendarEl) {
        var calendar = new FullCalendar.Calendar(calendarEl, {
            initialView: 'dayGridMonth',
            firstDay: '1',
            locale: 'pl',
            height: 'auto',

            displayEventTime: false,

            events: function (fetchInfo, successCallback, failureCallback) {
                Promise.all([
                    fetch('/get_current_reservations').then(response => response.json()),
                    fetch('/get_old_reservations').then(response => response.json())
                ])
                    .then(dataArray => {
                        const [currentReservations, oldReservations] = dataArray;

                        const eventColors = ["#2E7D32", "#880E4F", "#00695C", "#1565C0", "#4527A0", "#EF6C00"];

                        currentReservations.forEach((event, index) => {
                            const colorIndex = index % eventColors.length;
                            event.backgroundColor = eventColors[colorIndex];
                            event.borderColor = eventColors[colorIndex];
                        });

                        oldReservations.forEach(event => {
                            event.backgroundColor = 'gray';
                            event.borderColor = 'gray';
                        });

                        const allEvents = [...currentReservations, ...oldReservations];
                        successCallback(allEvents);
                    })
                    .catch(error => {
                        console.error('Error fetching events:', error);
                        failureCallback(error);
                    });
            },
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },
            buttonText: {
                today: 'Dziś',
                month: 'Miesiąc',
                week: 'Tydzień',
                day: 'Dzień'
            },

            eventClick: function (info) {
                const modal = document.getElementById('reservationModal');
                const props = info.event.extendedProps;

                document.getElementById('reservationGuest').innerText = props.firstName + ' ' + props.lastName;
                document.getElementById('reservationRoom').innerText = props.roomNumber;

                const startDate = new Date(info.event.start).toLocaleString('pl-PL', { dateStyle: 'long', timeStyle: 'short' });
                const endDate = info.event.end ? new Date(info.event.end).toLocaleString('pl-PL', { dateStyle: 'long', timeStyle: 'short' }) : 'Brak';

                document.getElementById('reservationStart').innerText = startDate;
                document.getElementById('reservationEnd').innerText = endDate;

                modal.style.display = "block";
            }
        });

        const closeModalBtn = document.getElementById('closeModal');
        if (closeModalBtn) {
            closeModalBtn.addEventListener('click', function () {
                document.getElementById('reservationModal').style.display = 'none';
            });
        }

        window.addEventListener('click', function (event) {
            const modal = document.getElementById('reservationModal');
            if (modal && event.target == modal) {
                modal.style.display = 'none';
            }
        });

        calendar.render();
    }
});

// AVAILABLE ROOMS - tylko jeśli elementy istnieją
document.addEventListener('DOMContentLoaded', () => {
    const dateFromInput = document.getElementById('dateFrom');
    const dateToInput = document.getElementById('dateTo');

    // Sprawdź czy elementy istnieją przed użyciem
    if (dateFromInput && dateToInput) {
        if (!dateFromInput.value) {
            const today = new Date();
            dateFromInput.value = today.toISOString().split('T')[0];
        }

        if (!dateToInput.value) {
            const tomorrow = new Date();
            tomorrow.setDate(tomorrow.getDate() + 1);
            dateToInput.value = tomorrow.toISOString().split('T')[0];
        }

        loadAvailableRooms();

        dateFromInput.addEventListener('change', loadAvailableRooms);
        dateToInput.addEventListener('change', loadAvailableRooms);
    }
});

async function loadAvailableRooms() {
    const dateFrom = document.getElementById('dateFrom');
    const dateTo = document.getElementById('dateTo');
    const roomSelect = document.getElementById('roomSelect');

    // Sprawdź czy elementy istnieją
    if (!dateFrom || !dateTo || !roomSelect) {
        return;
    }

    if (dateFrom.value && dateTo.value) {
        const response = await fetch(`/Reservations/GetAvailableRooms?dateFrom=${dateFrom.value}&dateTo=${dateTo.value}`);
        const rooms = await response.json();

        roomSelect.innerHTML = '<option value="">-- Wybierz pokój --</option>';

        rooms.forEach(room => {
            const option = document.createElement('option');
            option.value = room.idRoom;
            option.textContent = `${room.description} (Numer pokoju: ${room.roomNumber})`;
            roomSelect.appendChild(option);
        });
    }
}

// TABLE SORT
document.addEventListener("DOMContentLoaded", function () {
    const parsePolishDate = (dateStr) => {
        const parts = dateStr.match(/^(\d{2})\.(\d{2})\.(\d{4})(?: (\d{2}):(\d{2}))?$/);

        if (!parts) {
            return null;
        }

        const day = parts[1];
        const month = parts[2];
        const year = parts[3];
        const hour = parts[4] || '00';
        const minute = parts[5] || '00';

        return new Date(year, month - 1, day, hour, minute);
    };

    document.querySelectorAll("th").forEach((header, index) => {
        if (header.classList.contains('no-sort')) {
            return;
        }

        header.style.cursor = "pointer";

        header.addEventListener("click", () => {
            const table = header.closest("table");
            if (!table) return;
            
            const tbody = table.querySelector("tbody");
            if (!tbody) return;
            
            const rows = Array.from(tbody.querySelectorAll("tr"));
            const asc = (header.asc = !header.asc);

            document.querySelectorAll("th").forEach(th => {
                th.innerHTML = th.innerHTML.replace(/ ▲| ▼/g, "");
            });

            header.innerHTML += asc ? " ▲" : " ▼";

            rows.sort((a, b) => {
                if (!a.cells[index] || !b.cells[index]) return 0;
                
                const cellA = a.cells[index].innerText.trim();
                const cellB = b.cells[index].innerText.trim();

                const dateA = parsePolishDate(cellA);
                const dateB = parsePolishDate(cellB);

                if (dateA && dateB) {
                    return asc ? dateA - dateB : dateB - dateA;
                }

                const valA = parseFloat(cellA.replace(",", "."));
                const valB = parseFloat(cellB.replace(",", "."));

                if (!isNaN(valA) && !isNaN(valB)) {
                    return asc ? valA - valB : valB - valA;
                }

                return asc
                    ? cellA.localeCompare(cellB, "pl", { numeric: true, sensitivity: 'base' })
                    : cellB.localeCompare(cellA, "pl", { numeric: true, sensitivity: 'base' });
            });

            rows.forEach(row => tbody.appendChild(row));
        });
    });
});


