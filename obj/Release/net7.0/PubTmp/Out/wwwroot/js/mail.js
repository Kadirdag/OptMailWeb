function saveMail() {
    const subject = document.querySelector('input[placeholder="Mail konusunu giriniz"]').value;
    const body = document.getElementById('editor').innerHTML;
    const toSelect = document.getElementById('mail-to');
    const to = toSelect ? Array.from(toSelect.selectedOptions).map(opt => opt.value) : [];

    if (!subject || !body || to.length === 0) {
        alert("Lütfen konu, içerik ve alıcıları seçin!");
        return;
    }

    fetch('/Mail/SendMail', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ subject, body, to })
    })
        .then(r => r.json())
        .then(d => alert(d.message))
        .catch(err => alert("Hata: " + err));
}

