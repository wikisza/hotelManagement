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
    var calendarEl = document.getElementById('calendar');

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        firstDay: '1',
        events: function (fetchInfo, successCallback, failureCallback) {
            Promise.all([
                fetch('/get_current_reservations').then(response => response.json()),
                fetch('/get_old_reservations').then(response => response.json())
            ])
                .then(dataArray => {
                    const [currentReservations, oldReservations] = dataArray;

                    currentReservations.forEach((event, index) => {
                        const colorIndex = index % darkPalette.length;
                        event.backgroundColor = darkPalette[colorIndex];
                        event.borderColor = darkPalette[colorIndex];
                    });

                    oldReservations.forEach(event => {
                        event.backgroundColor = 'gray';
                        event.borderColor = event.backgroundColor;
                    });

                    const allEvents = [...currentReservations, ...oldReservations];
                    successCallback(allEvents);
                })
                .catch(error => {
                    console.error('Error fetching events:', error);
                    failureCallback(error);
                });
        },
        locale: 'pl',
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
            modal.className = 'modal';
            modal.style.position = 'fixed';
            modal.style.left = '50%';
            modal.style.top = '50%';
            modal.style.transform = 'translate(-50%, -50%)';
            modal.style.backgroundColor = '#fff';
            modal.style.padding = '20px';
            modal.style.boxShadow = '0 4px 8px rgba(0, 0, 0, 0.2)';
            modal.style.borderRadius = '8px';
            modal.style.zIndex = '1000';

            document.getElementById('reservationTitle').innerText = info.event.title;
            document.getElementById('reservationStart').innerText = new Date(info.event.start).toLocaleString();
            document.getElementById('reservationEnd').innerText = info.event.end ? new Date(info.event.end).toLocaleString() : 'Brak';
            document.getElementById('reservationDescription').innerText = info.event.extendedProps.description || 'Brak opisu';

            document.getElementById('reservationModal').style.display = "block";

            document.getElementById('closeModal').addEventListener('click', function () {
                document.getElementById('reservationModal').style.display = 'none';
            });

        }
    });

    calendar.render();



});



document.addEventListener('DOMContentLoaded', () => {
    const dateFromInput = document.getElementById('dateFrom');
    const dateToInput = document.getElementById('dateTo');

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
});

async function loadAvailableRooms() {
    const dateFrom = document.getElementById('dateFrom').value;
    const dateTo = document.getElementById('dateTo').value;

    if (dateFrom && dateTo) {
        const response = await fetch(`/Reservations/GetAvailableRooms?dateFrom=${dateFrom}&dateTo=${dateTo}`);
        const rooms = await response.json();

        const roomSelect = document.getElementById('roomSelect');
        roomSelect.innerHTML = '<option value="">-- Wybierz pokój --</option>';

        rooms.forEach(room => {
            const option = document.createElement('option');
            option.value = room.idRoom;
            option.textContent = `${room.description} (Numer pokoju: ${room.roomNumber})`;
            roomSelect.appendChild(option);
        });
    }
}


