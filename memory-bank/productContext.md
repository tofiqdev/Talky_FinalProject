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
- **Anında mesajlaşma**: Mesajlar gecikme olmadan iletilmeli
- **Sezgisel arayüz**: Kullanıcı kolayca navigasyon yapabilmeli
- **Responsive tasarım**: Mobil ve masaüstünde sorunsuz çalışmalı
- **Online durumu**: Kişilerin online/offline durumu görünmeli
- **Yazıyor göstergesi**: Karşı taraf yazarken bildirim gösterilmeli
- **Mesaj bildirimleri**: Yeni mesaj geldiğinde kullanıcı bilgilendirilmeli
