let isListening = false;
let rfidBuffer = "";
let selectedRoomId = null;
let selectedRoomNumber = null;
let readTimeout = null;

// Wybór pokoju
function selectRoom(roomId, roomNumber) {
    if (isListening) {
        cancelReading();
    }

    selectedRoomId = roomId;
    selectedRoomNumber = roomNumber;
    isListening = true;
    rfidBuffer = "";

    // Aktualizuj UI
    updateStatusBar(`Oczekiwanie na kartę dla pokoju ${roomNumber}...`, 'Zbliż kartę RFID do czytnika', 'waiting');
    
    // Podświetl wybrany pokój
    document.querySelectorAll('.room-card').forEach(card => {
        card.classList.remove('selected', 'access-granted', 'access-denied');
    });
    
    const selectedCard = document.querySelector(`[data-room-id='${roomId}']`);
    if (selectedCard) {
        selectedCard.classList.add('selected');
    }

    // Fokus na ukryte pole input
    const rfidInput = document.getElementById('rfidInput');
    rfidInput.value = '';
    rfidInput.focus();

    // Podświetl światełko przy drzwiach
    const light = document.getElementById(`light-${roomId}`);
    if (light) {
        light.classList.add('waiting');
    }

    // Timeout - anuluj po 30 sekundach bezczynności
    clearTimeout(readTimeout);
    readTimeout = setTimeout(() => {
        if (isListening) {
            cancelReading();
            showNotification('Przekroczono czas oczekiwania', 'warning');
        }
    }, 30000);
}

// Anuluj czytanie
function cancelReading() {
    isListening = false;
    selectedRoomId = null;
    selectedRoomNumber = null;
    rfidBuffer = "";
    clearTimeout(readTimeout);

    updateStatusBar('Wybierz pokój, aby rozpocząć', '', 'idle');

    document.querySelectorAll('.room-card').forEach(card => {
        card.classList.remove('selected');
    });

    document.querySelectorAll('.door-light').forEach(light => {
        light.classList.remove('waiting', 'granted', 'denied');
    });

    const rfidInput = document.getElementById('rfidInput');
    rfidInput.value = '';
    rfidInput.blur();
}

// Aktualizacja paska statusu
function updateStatusBar(statusText, hintText, state) {
    const statusEl = document.getElementById('rfidStatus');
    const hintEl = document.getElementById('rfidHint');
    const cancelBtn = document.getElementById('cancelBtn');
    const statusBar = document.getElementById('rfidStatusBar');

    if (statusEl) statusEl.textContent = statusText;
    if (hintEl) hintEl.textContent = hintText;

    statusBar.className = 'rfid-status-bar ' + state;
    
    if (state === 'waiting') {
        cancelBtn.style.display = 'block';
    } else {
        cancelBtn.style.display = 'none';
    }
}

// Sprawdź dostęp
async function checkAccess(roomId, roomNumber, keyCode) {
    updateStatusBar(`Weryfikacja karty dla pokoju ${roomNumber}...`, '', 'checking');

    try {
        const response = await fetch(`/Configuration/CheckAccess?roomId=${roomId}&keyCode=${encodeURIComponent(keyCode)}`);
        const result = await response.json();

        const card = document.querySelector(`[data-room-id='${roomId}']`);
        const door = document.getElementById(`door-${roomId}`);
        const light = document.getElementById(`light-${roomId}`);
        const resultEl = document.getElementById(`result-${roomId}`);

        // Usuń stan oczekiwania
        light.classList.remove('waiting');

        if (result.success) {
            // DOSTĘP PRZYZNANY
            updateStatusBar(`✅ ${result.message}`, `Pokój ${roomNumber}`, 'success');
            
            card.classList.add('access-granted');
            light.classList.add('granted');
            door.classList.add('opening');
            
            if (resultEl) {
                resultEl.innerHTML = '<span class="success-msg">✓ Dostęp przyznany</span>';
                resultEl.classList.add('show');
            }

            // Animacja otwierania drzwi
            setTimeout(() => {
                door.classList.remove('opening');
                door.classList.add('open');
            }, 500);

            // Zamknij drzwi po 3 sekundach
            setTimeout(() => {
                door.classList.remove('open');
                light.classList.remove('granted');
                card.classList.remove('access-granted');
                if (resultEl) resultEl.classList.remove('show');
                updateStatusBar('Wybierz pokój, aby rozpocząć', '', 'idle');
            }, 3000);

        } else {
            // DOSTĘP ODMÓWIONY
            updateStatusBar(`❌ ${result.message}`, `Pokój ${roomNumber}`, 'error');
            
            card.classList.add('access-denied');
            light.classList.add('denied');
            door.classList.add('denied');
            
            if (resultEl) {
                resultEl.innerHTML = '<span class="error-msg">✗ Odmowa dostępu</span>';
                resultEl.classList.add('show');
            }

            // Usuń efekt po 2 sekundach
            setTimeout(() => {
                door.classList.remove('denied');
                light.classList.remove('denied');
                card.classList.remove('access-denied');
                if (resultEl) resultEl.classList.remove('show');
                updateStatusBar('Wybierz pokój, aby rozpocząć', '', 'idle');
            }, 2000);
        }

    } catch (error) {
        console.error('Błąd podczas sprawdzania dostępu:', error);
        updateStatusBar('❌ Błąd połączenia', 'Spróbuj ponownie', 'error');
        setTimeout(() => {
            updateStatusBar('Wybierz pokój, aby rozpocząć', '', 'idle');
        }, 2000);
    }

    // Reset
    isListening = false;
    selectedRoomId = null;
    selectedRoomNumber = null;
    rfidBuffer = "";
}

// Pokazanie powiadomienia
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.textContent = message;
    document.body.appendChild(notification);

    setTimeout(() => notification.classList.add('show'), 10);
    setTimeout(() => {
        notification.classList.remove('show');
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// Obsługa wejścia z klawiatury (RFID reader)
document.addEventListener('DOMContentLoaded', function() {
    const rfidInput = document.getElementById('rfidInput');

    // Obsługa wpisywania
    rfidInput.addEventListener('input', function(e) {
        if (isListening) {
            rfidBuffer = this.value;
            
            // Auto-submit po osiągnięciu minimalnej długości
            if (rfidBuffer.length >= 8) {
                setTimeout(() => {
                    if (rfidBuffer.trim().length > 0) {
                        const cardCode = rfidBuffer.trim();
                        checkAccess(selectedRoomId, selectedRoomNumber, cardCode);
                        this.value = '';
                    }
                }, 100);
            }
        }
    });

    // Obsługa Enter
    rfidInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter' && isListening) {
            e.preventDefault();
            const cardCode = rfidBuffer.trim();
            if (cardCode.length > 0) {
                checkAccess(selectedRoomId, selectedRoomNumber, cardCode);
                this.value = '';
            }
        }
    });

    // Utrzymuj fokus na polu input podczas oczekiwania
    document.addEventListener('click', function(e) {
        if (isListening && e.target !== rfidInput) {
            setTimeout(() => rfidInput.focus(), 10);
        }
    });

    // Zapobiegaj utracie fokusa
    rfidInput.addEventListener('blur', function() {
        if (isListening) {
            setTimeout(() => this.focus(), 10);
        }
    });
});
