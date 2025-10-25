document.addEventListener("DOMContentLoaded", () => {
    const dateFromInput = document.getElementById("dateFrom");
    const dateToInput = document.getElementById("dateTo");
    const status = document.getElementById("rfidStatus");
    const assignButton = document.getElementById("assignCardBtn");
    const keyInput = document.getElementById("keyCodeInput");

    // --- Walidacja dat ---
    const today = new Date();
    today.setHours(14, 0, 0, 0);

    // Ustaw minimalną datę rozpoczęcia
    dateFromInput.min = today.toISOString().slice(0, 16);

    // Zapamiętujemy poprzednie poprawne wartości
    let lastValidFrom = dateFromInput.value;
    let lastValidTo = dateToInput.value;

    // Aktualizuj minimalną datę zakończenia przy zmianie "od"
    dateFromInput.addEventListener("change", () => {
        const startDate = new Date(dateFromInput.value);
        const nextDay = new Date(startDate);
        nextDay.setDate(startDate.getDate() + 1);
        nextDay.setHours(10, 0, 0, 0);

        dateToInput.min = nextDay.toISOString().slice(0, 16);

        // Jeśli data rozpoczęcia jest wcześniejsza niż dziś — wróć do poprzedniej
        if (startDate < today) {
            alert("Nie można wybrać daty rozpoczęcia wcześniejszej niż dzisiaj.");
            dateFromInput.value = lastValidFrom;
            return;
        }

        // Jeśli zakończenie jest wcześniejsze niż dzień po rozpoczęciu — przywróć poprzednią
        if (dateToInput.value && new Date(dateToInput.value) < nextDay) {
            alert("Data zakończenia nie może być wcześniejsza niż dzień po rozpoczęciu pobytu.");
            dateToInput.value = lastValidTo;
            return;
        }

        lastValidFrom = dateFromInput.value;
    });

    // Walidacja przy zmianie "do"
    dateToInput.addEventListener("change", () => {
        const startDate = new Date(dateFromInput.value);
        const endDate = new Date(dateToInput.value);

        if (endDate <= startDate) {
            alert("Data zakończenia musi być późniejsza niż data rozpoczęcia.");
            dateToInput.value = lastValidTo; 
            return;
        }

        lastValidTo = dateToInput.value;
    });

    // --- Obsługa karty RFID ---
    let listening = false;
    let buffer = "";

    assignButton.addEventListener("click", () => {
        listening = true;
        buffer = "";
        keyInput.value = "";
        status.textContent = "Przyłóż kartę do czytnika...";
        assignButton.classList.add("listening");
        assignButton.classList.remove("assigned");
    });

    document.addEventListener("keydown", (e) => {
        if (!listening) return;

        if (e.key === "Enter") {
            listening = false;
            keyInput.value = buffer.trim();
            status.textContent = "Karta przypisana ✅";
            buffer = "";
            assignButton.classList.remove("listening");
            assignButton.classList.add("assigned");
            e.preventDefault();
            return;
        }

        buffer += e.key;
    });
});





let selectedRow = null;
let selectedReservationId = null;

document.querySelectorAll('#reservationsTable tbody tr').forEach(row => {
    row.addEventListener('click', () => {
        if (selectedRow) selectedRow.classList.remove('selected');

        row.classList.add('selected');
        selectedRow = row;
        selectedReservationId = row.getAttribute('data-reservation-id');

        document.getElementById('detailsBtn').disabled = false;
        const editBtn = document.getElementById('editBtn');
        const deleteBtn = document.getElementById('deleteBtn');
        if (editBtn) editBtn.disabled = false;
        if (deleteBtn) deleteBtn.disabled = false;
    });
});

document.getElementById('detailsBtn').addEventListener('click', () => {
    if (selectedReservationId)
        window.location.href = `/Reservations/Details/${selectedReservationId}`;
});

const editBtn = document.getElementById('editBtn');
if (editBtn) {
    editBtn.addEventListener('click', () => {
        if (selectedReservationId)
            window.location.href = `/Reservations/Edit/${selectedReservationId}`;
    });
}

const deleteBtn = document.getElementById('deleteBtn');
if (deleteBtn) {
    deleteBtn.addEventListener('click', () => {
        if (selectedReservationId && confirm("Czy na pewno chcesz usunąć ten pokój?")) {
            window.location.href = `/Reservations/Delete/${selectedReservationId}`;
        }
    });
}