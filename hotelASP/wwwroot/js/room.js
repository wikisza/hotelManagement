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

const deleteBtn = document.getElementById('deleteBtn');
if (deleteBtn) {
    deleteBtn.addEventListener('click', () => {
        if (selectedRoomId && confirm("Czy na pewno chcesz usunąć ten pokój?")) {
            window.location.href = `/Rooms/DeleteRoom/${selectedRoomId}`;
        }
    });
}