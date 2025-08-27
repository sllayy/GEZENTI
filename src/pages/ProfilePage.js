import React, { useState, useEffect } from 'react';

const ProfilePage = () => {
  // Gelen veriyi saklamak için bir state oluşturuyoruz
  const [profileData, setProfileData] = useState(null);
  // Yükleme durumunu yönetmek için bir state
  const [loading, setLoading] = useState(true);
  // Hata durumunu yönetmek için bir state
  const [error, setError] = useState('');

  useEffect(() => {
    // Sayfa ilk yüklendiğinde çalışacak kod bloğu

    // 1. Adım: Tarayıcı hafızasından token'ı al
    const token = localStorage.getItem("authToken");

    // Eğer token yoksa, kullanıcı giriş yapmamış demektir.
    // Onu login sayfasına geri yolla (bu satır projeye göre değişebilir).
    if (!token) {
      setError("Lütfen giriş yapın.");
      setLoading(false);
      // window.location.href = '/login'; // Gerekirse bu satır açılabilir
      return;
    }

    // 2. Adım: Senin backend'ine istek at
    fetch('/api/profile', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        // EN ÖNEMLİ KISIM: Token'ı Authorization başlığına ekliyoruz
        'Authorization': `Bearer ${token}`
      }
    })
    .then(response => {
      // Eğer cevap 401 (Unauthorized) gibi bir hata ise, token geçersiz demektir.
      if (!response.ok) {
        throw new Error('Yetkilendirme hatası. Lütfen tekrar giriş yapın.');
      }
      return response.json();
    })
    .then(data => {
      // 3. Adım: Gelen veriyi state'e kaydet
      setProfileData(data);
      setLoading(false);
    })
    .catch(err => {
      console.error("Profil verisi çekilirken hata:", err);
      setError(err.message);
      setLoading(false);
    });

  }, []); // Boş array, bu kodun sadece component ilk yüklendiğinde çalışmasını sağlar

  // Arayüzü oluşturma
  if (loading) {
    return <div>Profil bilgileri yükleniyor...</div>;
  }

  if (error) {
    return <div>Hata: {error}</div>;
  }

  return (
    <div>
      <h1>Profil Sayfanız</h1>
      {profileData ? (
        <div>
          <p><strong>İsim:</strong> {profileData.name}</p>
          <p><strong>Email:</strong> {profileData.email}</p>
          <p><strong>Bütçe:</strong> {profileData.budget}</p>
          <p><strong>Maksimum Yürüme Mesafesi:</strong> {profileData.max_walk} metre</p>
        </div>
      ) : (
        <p>Profil bilgileri bulunamadı.</p>
      )}
    </div>
  );
};

export default ProfilePage;