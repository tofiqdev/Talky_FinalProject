# Product Context

## Neden Bu Proje Var?
Talky, kullanıcıların gerçek zamanlı olarak iletişim kurabilecekleri modern bir mesajlaşma platformu sağlamak için geliştirilmektedir.

## Çözdüğü Problemler
- **Gerçek zamanlı iletişim** → SignalR WebSocket teknolojisi
- **Güvenli kimlik doğrulama** → JWT token sistemi
- **Mesaj geçmişi** → Database ile kalıcı saklama
- **Kullanıcı deneyimi** → Modern, hızlı ve responsive arayüz
- **Ölçeklenebilirlik** → .NET backend ile performanslı API

## Nasıl Çalışmalı

### Kullanıcı Akışı
1. Kullanıcı kayıt olur veya giriş yapar
2. Kişi listesini görür (online/offline durumları ile)
3. Bir kişiye tıklayarak sohbet başlatır
4. Mesajlar gerçek zamanlı olarak iletilir
5. Mesaj geçmişi saklanır ve tekrar yüklenebilir

### Teknik Akış
1. Frontend SignalR hub'a bağlanır
2. JWT token ile kimlik doğrulama yapılır
3. Mesajlar WebSocket üzerinden iletilir
4. Backend mesajları database'e kaydeder
5. Alıcıya gerçek zamanlı bildirim gönderilir

## Kullanıcı Deneyimi Hedefleri
- ✅ **Anında mesajlaşma**: Mesajlar SignalR ile gecikme olmadan iletiliyor
- ✅ **Sezgisel arayüz**: Tab-based navigation ile kolay kullanım
- ✅ **Responsive tasarım**: Mobil ve masaüstünde sorunsuz çalışıyor
- ✅ **Online durumu**: Kişilerin online/offline durumu real-time görünüyor
- ✅ **Grup sohbetleri**: Grup oluşturma, üye yönetimi, yetkilendirme
- ✅ **Ses mesajları**: Basılı tut & kaydet özelliği ile ses gönderimi
- ⏳ **Yazıyor göstergesi**: Karşı taraf yazarken bildirim (opsiyonel)
- ⏳ **Mesaj bildirimleri**: Push notifications (opsiyonel)
