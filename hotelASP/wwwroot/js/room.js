let selectedRow = null;
let selectedRoomId = null;

document.querySelectorAll('#roomsTable tbody tr').forEach(row => {
    row.addEventListener('click', () => {
        // usuń zaznaczenie z poprzedniego
        if (selectedRow) selectedRow.classList.remove('selected');

        // zaznacz nowy
        row.classList.add('selected');
        selectedRow = row;
        selectedRoomId = row.getAttribute('data-room-id');

        // aktywuj przyciski
        document.getElementById('detailsBtn').disabled = false;
        const editBtn = document.getElementById('editBtn');
        const deleteBtn = document.getElementById('deleteBtn');
        if (editBtn) editBtn.disabled = false;
        if (deleteBtn) deleteBtn.disabled = false;
    });
});

// Obsługa przycisków akcji
document.getElementById('detailsBtn').addEventListener('click', () => {
    if (selectedRoomId)
        window.location.href = `/Rooms/Details/${selectedRoomId}`;
});

const editBtn = document.getElementById('editBtn');
if (editBtn) {
    editBtn.addEventListener('click', () => {
        if (selectedRoomId)
            window.location.href = `/Rooms/UpdateRoom/${selectedRoomId}`;
    });
}


// Funkcja zamykająca modal
function closeDeleteModal() {
    const modal = document.getElementById("deleteModal");
    if (modal) {
        modal.style.display = "none";
    }
}

// Zamykanie modala po kliknięciu w tło (obszar poza okienkiem)
window.onclick = function (event) {
    const deleteModal = document.getElementById("deleteModal");
    if (event.target === deleteModal) {
        closeDeleteModal();
    }
}

const deleteBtn = document.getElementById('deleteBtn');
if (deleteBtn) {
    deleteBtn.addEventListener('click', () => {
        // Sprawdź czy wybrano pokój
        if (selectedRoomId) {
            // Ustaw ID pokoju w ukrytym polu formularza w modalu
            document.getElementById('deleteRoomId').value = selectedRoomId;

            // Pokaż modal, ustawiając style flex, aby wyśrodkować go tak jak w przykładzie
            const modal = document.getElementById("deleteModal");
            modal.style.display = "flex";
            modal.style.alignItems = "center";
            modal.style.justifyContent = "center";
        }
    });
}