let pomodoroInterval = null;

function getPomodoroState() {
    let savedTime = localStorage.getItem("pomodoroTime");
    let isRunning = localStorage.getItem("pomodoroRunning");
    let lastUpdated = localStorage.getItem("pomodoroLastUpdated");

    let time = savedTime ? parseInt(savedTime) : 25 * 60;

    if (isRunning === "true" && lastUpdated) {
        let now = Math.floor(Date.now() / 1000);
        let elapsed = now - parseInt(lastUpdated);
        time = Math.max(time - elapsed, 0);
        localStorage.setItem("pomodoroTime", time);
        localStorage.setItem("pomodoroLastUpdated", now);
    }

    return {
        time: time,
        isRunning: isRunning === "true"
    };
}

function savePomodoroState(time, isRunning) {
    localStorage.setItem("pomodoroTime", time);
    localStorage.setItem("pomodoroRunning", isRunning);
    localStorage.setItem("pomodoroLastUpdated", Math.floor(Date.now() / 1000));
}

function formatTime(seconds) {
    let minutes = Math.floor(seconds / 60);
    let secs = seconds % 60;

    return String(minutes).padStart(2, "0") + " : " + String(secs).padStart(2, "0");
}

function updatePomodoroDisplay() {
    let state = getPomodoroState();
    let displayText = formatTime(state.time);

    let mainDisplay = document.getElementById("mainTimerDisplay");
    let floatingDisplay = document.getElementById("floatingTimerDisplay");

    if (mainDisplay) {
        mainDisplay.innerText = displayText;
    }

    if (floatingDisplay) {
        floatingDisplay.innerText = displayText;
    }
}

function startPomodoro() {
    let state = getPomodoroState();

    if (state.time <= 0) {
        state.time = 25 * 60;
    }

    savePomodoroState(state.time, true);
    runPomodoroInterval();
}

function pausePomodoro() {
    let state = getPomodoroState();
    savePomodoroState(state.time, false);

    clearInterval(pomodoroInterval);
    pomodoroInterval = null;

    updatePomodoroDisplay();
}

function resetPomodoro() {
    savePomodoroState(25 * 60, false);

    clearInterval(pomodoroInterval);
    pomodoroInterval = null;

    updatePomodoroDisplay();
}

function runPomodoroInterval() {
    if (pomodoroInterval !== null) {
        return;
    }

    pomodoroInterval = setInterval(function () {
        let state = getPomodoroState();

        if (!state.isRunning) {
            clearInterval(pomodoroInterval);
            pomodoroInterval = null;
            return;
        }

        if (state.time <= 0) {
            savePomodoroState(0, false);
            clearInterval(pomodoroInterval);
            pomodoroInterval = null;
            alert("Pomodoro session completed.");
            updatePomodoroDisplay();
            return;
        }

        let newTime = state.time - 1;
        savePomodoroState(newTime, true);
        updatePomodoroDisplay();

    }, 1000);
}

function toggleMusic() {
    const player = document.getElementById("globalLofiPlayer");

    if (!player) return;

    if (player.paused) {
        player.play().then(() => {
            localStorage.setItem("musicPlaying", "true");
        }).catch(() => {
            console.log("Music autoplay blocked until user clicks play.");
        });
    } else {
        localStorage.setItem("musicPlaying", "false");
        player.pause();
    }
}
function changeMusicVolume(value) {
    const player = document.getElementById("globalLofiPlayer");

    if (!player) return;

    player.volume = parseFloat(value);
    localStorage.setItem("musicVolume", value);
}

function setupMusicVolumeSlider() {
    const slider = document.getElementById("musicVolume");
    const savedVolume = localStorage.getItem("musicVolume");

    if (slider && savedVolume !== null) {
        slider.value = savedVolume;
    }
}
function saveMusicProgress() {
    const player = document.getElementById("globalLofiPlayer");

    if (!player) return;

    localStorage.setItem("musicCurrentTime", player.currentTime);
    localStorage.setItem("musicVolume", player.volume);
}

function restoreMusicProgress() {
    const player = document.getElementById("globalLofiPlayer");

    if (!player) return;

    const savedTime = localStorage.getItem("musicCurrentTime");
    const savedVolume = localStorage.getItem("musicVolume");
    const musicPlaying = localStorage.getItem("musicPlaying");

    if (savedVolume !== null) {
        player.volume = parseFloat(savedVolume);
    }

    player.addEventListener("loadedmetadata", function () {
        if (savedTime !== null) {
            player.currentTime = parseFloat(savedTime);
        }

        if (musicPlaying === "true") {
            player.play().catch(() => {
                console.log("Autoplay blocked after page change.");
            });
        }
    });
}

window.addEventListener("beforeunload", saveMusicProgress);


document.addEventListener("DOMContentLoaded", function () {
    restoreMusicProgress();
    setupMusicVolumeSlider();
    updatePomodoroDisplay();
    setupChatCooldown();

    let state = getPomodoroState();

    if (state.isRunning) {
        runPomodoroInterval();
    }
});

function flipCard(card) {
    card.classList.toggle("flipped");
}

// chat

function lockChatButton() {
    const button = document.getElementById("sendChatButton");
    const input = document.getElementById("chatMessageInput");

    if (!button || !input) {
        return true;
    }

    button.disabled = true;
    input.readOnly = true;
    button.innerText = "Wait...";

    localStorage.setItem("chatCooldownEnd", Date.now() + 10000);

    return true;
}

function setupChatCooldown() {
    const button = document.getElementById("sendChatButton");
    const input = document.getElementById("chatMessageInput");
    const cooldownText = document.getElementById("cooldownText");

    if (!button || !input || !cooldownText) {
        return;
    }

    function updateCooldown() {
        const cooldownEnd = parseInt(localStorage.getItem("chatCooldownEnd") || "0");
        const remaining = Math.ceil((cooldownEnd - Date.now()) / 1000);

        if (remaining > 0) {
            button.disabled = true;
            input.readOnly = true;
            button.innerText = remaining + "s";
            cooldownText.innerText = "Slow mode enabled. You can send again in " + remaining + " seconds.";
        } else {
            button.disabled = false;
            input.readOnly = false;
            button.innerText = "Send";
            cooldownText.innerText = "";
        }
    }

    updateCooldown();
    setInterval(updateCooldown, 1000);
}
function showDeletedPostPopup() {
    alert("This forum post has been deleted.");
}

/*Mininimize the floating timer*/
document.addEventListener("DOMContentLoaded", function () {
    const pomodoroBox = document.getElementById("floatingPomodoro");
    const dragHandle = document.getElementById("pomodoroDragHandle");
    const minimizeBtn = document.getElementById("togglePomodoroMinimize");

    if (!pomodoroBox || !dragHandle || !minimizeBtn) {
        return;
    }

    const savedLeft = localStorage.getItem("floatingPomodoroLeft");
    const savedTop = localStorage.getItem("floatingPomodoroTop");
    const savedMinimized = localStorage.getItem("floatingPomodoroMinimized") === "true";

    if (savedLeft && savedTop) {
        pomodoroBox.style.left = savedLeft;
        pomodoroBox.style.top = savedTop;
        pomodoroBox.style.right = "auto";
        pomodoroBox.style.bottom = "auto";
    }

    if (savedMinimized) {
        pomodoroBox.classList.add("minimized");
        minimizeBtn.textContent = "+";
        minimizeBtn.title = "Expand timer";
    }

    minimizeBtn.addEventListener("click", function (event) {
        event.stopPropagation();

        pomodoroBox.classList.toggle("minimized");

        const isMinimized = pomodoroBox.classList.contains("minimized");

        localStorage.setItem("floatingPomodoroMinimized", isMinimized ? "true" : "false");

        minimizeBtn.textContent = isMinimized ? "+" : "−";
        minimizeBtn.title = isMinimized ? "Expand timer" : "Minimize timer";
    });

    let isDragging = false;
    let offsetX = 0;
    let offsetY = 0;

    dragHandle.addEventListener("mousedown", function (event) {
        if (event.target === minimizeBtn) {
            return;
        }

        isDragging = true;

        const rect = pomodoroBox.getBoundingClientRect();

        offsetX = event.clientX - rect.left;
        offsetY = event.clientY - rect.top;

        pomodoroBox.style.left = rect.left + "px";
        pomodoroBox.style.top = rect.top + "px";
        pomodoroBox.style.right = "auto";
        pomodoroBox.style.bottom = "auto";

        document.body.style.userSelect = "none";
    });

    document.addEventListener("mousemove", function (event) {
        if (!isDragging) {
            return;
        }

        let newLeft = event.clientX - offsetX;
        let newTop = event.clientY - offsetY;

        const maxLeft = window.innerWidth - pomodoroBox.offsetWidth;
        const maxTop = window.innerHeight - pomodoroBox.offsetHeight;

        newLeft = Math.max(0, Math.min(newLeft, maxLeft));
        newTop = Math.max(0, Math.min(newTop, maxTop));

        pomodoroBox.style.left = newLeft + "px";
        pomodoroBox.style.top = newTop + "px";
    });

    document.addEventListener("mouseup", function () {
        if (!isDragging) {
            return;
        }

        isDragging = false;
        document.body.style.userSelect = "";

        localStorage.setItem("floatingPomodoroLeft", pomodoroBox.style.left);
        localStorage.setItem("floatingPomodoroTop", pomodoroBox.style.top);
    });

    dragHandle.addEventListener("touchstart", function (event) {
        if (event.target === minimizeBtn) {
            return;
        }

        const touch = event.touches[0];
        const rect = pomodoroBox.getBoundingClientRect();

        isDragging = true;
        offsetX = touch.clientX - rect.left;
        offsetY = touch.clientY - rect.top;

        pomodoroBox.style.left = rect.left + "px";
        pomodoroBox.style.top = rect.top + "px";
        pomodoroBox.style.right = "auto";
        pomodoroBox.style.bottom = "auto";
    });

    document.addEventListener("touchmove", function (event) {
        if (!isDragging) {
            return;
        }

        const touch = event.touches[0];

        let newLeft = touch.clientX - offsetX;
        let newTop = touch.clientY - offsetY;

        const maxLeft = window.innerWidth - pomodoroBox.offsetWidth;
        const maxTop = window.innerHeight - pomodoroBox.offsetHeight;

        newLeft = Math.max(0, Math.min(newLeft, maxLeft));
        newTop = Math.max(0, Math.min(newTop, maxTop));

        pomodoroBox.style.left = newLeft + "px";
        pomodoroBox.style.top = newTop + "px";

        event.preventDefault();
    }, { passive: false });

    document.addEventListener("touchend", function () {
        if (!isDragging) {
            return;
        }

        isDragging = false;

        localStorage.setItem("floatingPomodoroLeft", pomodoroBox.style.left);
        localStorage.setItem("floatingPomodoroTop", pomodoroBox.style.top);
    });
});