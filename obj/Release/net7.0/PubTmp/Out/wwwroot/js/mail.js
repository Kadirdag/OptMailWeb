// Kaydet butonuna basılınca tetiklenecek fonksiyon
function saveSelectedUsers() {
    const selectedUsers = [];

    $(".user-checkbox:checked").each(function () {
        selectedUsers.push({
            adSoyad: $(this).data("adsoyad"),
            mail: $(this).data("mail"),
            telefon: $(this).data("telefon")
        });
    });

    if (selectedUsers.length === 0) {
        alert("Lütfen en az bir kullanıcı seçiniz!");
        return;
    }

    console.log("Seçilen kullanıcılar:", selectedUsers);

    // AJAX ile server'a göndermek için örnek
    $.ajax({
        url: "/Mail/SaveSelectedUsers",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(selectedUsers),
        success: function (res) {
            alert("Seçilen kullanıcılar başarıyla kaydedildi.");
        },
        error: function (err) {
            alert("Kaydetme sırasında hata oluştu!");
            console.error(err);
        }
    });
}

// Kaydet butonuna event bağlama
$(document).on("click", "#btnSave", saveSelectedUsers);
