/**
 * Lógica del Chatbot - ClinPiura
 * Gestiona el envío de mensajes al controlador Chat/Consultar
 */

async function enviarMensaje() {
    const input = document.getElementById('chatInput');
    const container = document.getElementById('chatMessages');
    const mensaje = input.value.trim();

    if (!mensaje) return;

    agregarMensajeAlChat(mensaje, 'msg-user');
    input.value = '';

    // Crear indicador de carga
    const typingDiv = document.createElement('div');
    typingDiv.className = 'typing-indicator';
    typingDiv.innerHTML = '<div class="dot"></div><div class="dot"></div><div class="dot"></div>';
    container.appendChild(typingDiv);
    container.scrollTop = container.scrollHeight;

    try {
        const res = await fetch('/Chat/Consultar', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(mensaje)
        });

        const data = await res.json();

        // Eliminar indicador y mostrar respuesta
        container.removeChild(typingDiv);
        agregarMensajeAlChat(data.respuesta, 'msg-bot');

    } catch (err) {
        container.removeChild(typingDiv);
        agregarMensajeAlChat('Error al conectar con el asistente.', 'msg-bot');
    }
}

function agregarMensajeAlChat(texto, clase) {
    const container = document.getElementById('chatMessages');
    const msgDiv = document.createElement('div');
    msgDiv.className = `msg-bubble ${clase}`;
    msgDiv.innerHTML = texto;
    container.appendChild(msgDiv);
    container.scrollTop = container.scrollHeight;
}

document.getElementById('chatInput')?.addEventListener('keypress', (e) => {
    if (e.key === 'Enter') enviarMensaje();
});
