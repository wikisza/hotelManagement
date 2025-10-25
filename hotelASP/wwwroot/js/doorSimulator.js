let listening = false;
let buffer = "";
let currentRoomId = null;

function openDoor(roomId) {
    if (listening) return;

    currentRoomId = roomId;
    listening = true;
    buffer = "";

    const statusEl = document.getElementById("rfidStatus");
    if (statusEl) {
        statusEl.textContent = "Przyłóż kartę do czytnika...";
    }

    const door = document.querySelector(`.door[data-room-id='${roomId}'] .door-panel`);
    door.classList.add("waiting");
    document.addEventListener("keydown", handleRFIDInput);
}

function handleRFIDInput(e) {
    if (!listening) return;

    if (e.key === "Enter") {
        listening = false;
        document.removeEventListener("keydown", handleRFIDInput);
        const keyCode = buffer.trim();
        buffer = "";
        checkAccess(currentRoomId, keyCode);
        return;
    }

    buffer += e.key;
}

async function checkAccess(roomId, keyCode) {
    try {
        const response = await fetch(`/Configuration/CheckAccess?roomId=${roomId}&keyCode=${keyCode}`);
        const result = await response.json();
        const door = document.querySelector(`.door[data-room-id='${roomId}'] .door-panel`);

        door.classList.remove("waiting");

        if (result.success) {
            door.classList.add("open");
            setTimeout(() => {
                door.classList.remove("open");
            }, 3000);
        } else {
            door.classList.add("denied");
            setTimeout(() => {
                door.classList.remove("denied");
            }, 800);
        }
    } catch (error) {
        console.error("Błąd podczas sprawdzania dostępu:", error);
    }
}
