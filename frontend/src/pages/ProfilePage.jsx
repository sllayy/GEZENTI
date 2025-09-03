import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faEdit, faPlus, faSave, faKey, faTrash } from "@fortawesome/free-solid-svg-icons";
import { authAPI } from "../services/api";
import PastRoutesList from "../components/PastRoutes/PastRoutesList";

const ProfilePage = ({ avatarIndex, setAvatarIndex, setIsLoggedIn, setUserName }) => {
  const navigate = useNavigate();

  const [activeTab, setActiveTab] = useState("history");
  const [profileData, setProfileData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const avatars = [
      "/assets/avatar1.png",
      "/assets/avatar2.png",
      "/assets/avatar3.png",
      "/assets/avatar4.png",
      "/assets/avatar5.png",
      "/assets/avatar6.png",
      "/assets/avatar7.png"
    ];
  const [aboutMe, setAboutMe] = useState("");

  const [activeForm, setActiveForm] = useState(null);
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmNewPassword, setConfirmNewPassword] = useState("");
  const [deletePassword, setDeletePassword] = useState("");
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [statusMessage, setStatusMessage] = useState("");
  const [statusType, setStatusType] = useState("");

  useEffect(() => {
    authAPI
      .getMe()
      .then((res) => {
        setProfileData(res);
        setFirstName(res.firstName);
        setLastName(res.lastName);
        setAvatarIndex(res.avatarIndex ?? 0);
        setAboutMe(res.aboutMe ?? "");
        setLoading(false);
      })
      .catch(() => {
        setError("Profil bilgileri alınamadı. Lütfen tekrar giriş yapın.");
        setLoading(false);
      });
  }, []);

    useEffect(() => {
        if (statusMessage) {
            const timer = setTimeout(() => {
                setStatusMessage("");
                setStatusType("");
            }, 5000);

            return () => clearTimeout(timer);
        }
    }, [statusMessage]);

    const handleUpdateProfile = async () => {
        try {
            const updated = await authAPI.updateProfile({ firstName, lastName, avatarIndex, aboutMe });
            setProfileData(updated);

            localStorage.setItem("avatarIndex", avatarIndex);
            setAvatarIndex(avatarIndex);

            setStatusMessage("Profil başarıyla güncellendi");
            setStatusType("success");
            setActiveForm(null);
        } catch {
            setStatusMessage("Profil güncellenemedi");
            setStatusType("error");
        }
    };

    const handleChangePassword = async () => {
        if (newPassword !== confirmNewPassword) {
            setStatusMessage("Yeni şifreler uyuşmuyor");
            setStatusType("error");
            return;
        }

        const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,12}$/;
        if (!passwordRegex.test(newPassword)) {
            setStatusMessage("Şifre 8-12 karakter olmalı ve en az 1 büyük harf, 1 küçük harf, 1 rakam ve 1 sembol içermelidir.");
            setStatusType("error");
            return;
        }

        try {
            await authAPI.changePassword({ oldPassword, newPassword });
            setStatusMessage("Şifre başarıyla güncellendi ");
            setStatusType("success");
            setOldPassword("");
            setNewPassword("");
            setConfirmNewPassword("");
            setActiveForm(null);
        } catch {
            setStatusMessage("Şifre güncellenemedi");
            setStatusType("error");
        }
    };

    const handleDeleteAccount = async () => {
        try {
            await authAPI.deleteAccount({ password: deletePassword });
            localStorage.removeItem("jwtToken");
            localStorage.removeItem("userName");

            setIsLoggedIn(false);
            setUserName(null);

            setStatusMessage("Hesabınız silindi ❌");
            setStatusType("success");

            setTimeout(() => navigate("/login"), 2000);
        } catch {
            setStatusMessage("Hesap silme başarısız ❌");
            setStatusType("error");
        }
    };

  const handleCreateRoute = () => navigate("/route-builder");

  if (loading) return <div>Profil yükleniyor...</div>;
  if (error) return <div>Hata: {error}</div>;

  const userName = profileData?.firstName || "User";

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      {/* Profil üst kısmı */}
      <div className="flex flex-col md:flex-row justify-between items-center p-6 bg-white rounded-xl shadow-md mb-6 space-y-4 md:space-y-0">
              <div className="flex items-center space-x-4">
                  <div className="w-24 h-24 rounded-full bg-gray-200 flex items-center justify-center overflow-hidden">
                      <img src={avatars[avatarIndex]} alt="Avatar" className="w-full h-full object-cover" />
                  </div>
                  <div>
                      <h2 className="text-2xl font-bold">
                          {profileData?.firstName} {profileData?.lastName}
                      </h2>
                      <p className="text-gray-500">{profileData?.aboutMe}</p>
                      <p className="text-gray-600">{profileData?.email}</p>
                  </div>
              </div>

        <div className="flex space-x-4 items-center">
          <button
            onClick={handleCreateRoute}
            className="flex items-center px-5 py-3 bg-blue-500 text-white rounded-lg shadow-lg hover:bg-blue-600 transition"
          >
            <FontAwesomeIcon icon={faPlus} className="mr-2" /> Rota Oluştur
          </button>
        </div>
      </div>

      {/* Hesap ayarları */}
          <div className="bg-white rounded-xl shadow-md p-6 mb-6 space-y-6">
              <h3 className="text-xl font-bold mb-4">Hesap Ayarları</h3>

              <div className="flex space-x-4 mb-4">
                  <button
                      onClick={() => setActiveForm(activeForm === "profile" ? null : "profile")}
                      className="flex items-center px-5 py-3 border border-gray-300 text-gray-700 rounded-lg shadow hover:bg-gray-100 transition"
                  >
                      <FontAwesomeIcon icon={faEdit} className="mr-2" /> Profili Güncelle
                  </button>

                  <button
                      onClick={() => setActiveForm(activeForm === "about" ? null : "about")}
                      className="flex items-center px-5 py-3 border border-gray-300 text-gray-700 rounded-lg shadow hover:bg-gray-100 transition"
                  >
                      <FontAwesomeIcon icon={faEdit} className="mr-2" /> Hakkında Güncelle
                  </button>

                  <button
                      onClick={() => setActiveForm(activeForm === "avatar" ? null : "avatar")}
                      className="flex items-center px-5 py-3 bg-purple-500 text-white rounded-lg shadow hover:bg-purple-600 transition"
                  >
                      Avatarı Değiştir
                  </button>

                  <button
                      onClick={() => setActiveForm(activeForm === "password" ? null : "password")}
                      className="flex items-center px-5 py-3 bg-yellow-500 text-white rounded-lg shadow hover:bg-yellow-600 transition"
                  >
                      <FontAwesomeIcon icon={faKey} className="mr-2" /> Şifreyi Değiştir
                  </button>

                  <button
                      onClick={() => setActiveForm(activeForm === "delete" ? null : "delete")}
                      className="flex items-center px-5 py-3 bg-red-500 text-white rounded-lg shadow hover:bg-red-600 transition"
                  >
                      <FontAwesomeIcon icon={faTrash} className="mr-2" /> Hesabı Sil
                  </button>
              </div>

              {/* Profil Güncelle */}
              {activeForm === "profile" && (
                  <div>
                      <input
                          type="text"
                          className="border p-2 rounded mr-2"
                          value={firstName}
                          onChange={(e) => setFirstName(e.target.value)}
                          placeholder="Ad"
                      />
                      <input
                          type="text"
                          className="border p-2 rounded mr-2"
                          value={lastName}
                          onChange={(e) => setLastName(e.target.value)}
                          placeholder="Soyad"
                      />
                      <button
                          onClick={handleUpdateProfile}
                          className="px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600"
                      >
                          <FontAwesomeIcon icon={faSave} className="mr-1" /> Kaydet
                      </button>
                  </div>
              )}

              {/* Şifreyi Değiştir */}
              {activeForm === "password" && (
                  <div>
                      <input
                          type="password"
                          className="border p-2 rounded mr-2"
                          value={oldPassword}
                          onChange={(e) => setOldPassword(e.target.value)}
                          placeholder="Eski Şifre"
                      />
                      <input
                          type="password"
                          className="border p-2 rounded mr-2"
                          value={newPassword}
                          onChange={(e) => setNewPassword(e.target.value)}
                          placeholder="Yeni Şifre"
                      />
                      <input
                          type="password"
                          className="border p-2 rounded mr-2"
                          value={confirmNewPassword}
                          onChange={(e) => setConfirmNewPassword(e.target.value)}
                          placeholder="Yeni Şifre (Tekrar)"
                      />
                      <button
                          onClick={handleChangePassword}
                          className="px-4 py-2 bg-yellow-500 text-white rounded hover:bg-yellow-600"
                      >
                          <FontAwesomeIcon icon={faKey} className="mr-1" /> Değiştir
                      </button>
                  </div>
              )}

              {/* Hesabı Sil */}
              {activeForm === "delete" && (
                  <div>
                      {!showDeleteConfirm ? (
                          <>
                              <input
                                  type="password"
                                  className="border p-2 rounded mr-2"
                                  value={deletePassword}
                                  onChange={(e) => setDeletePassword(e.target.value)}
                                  placeholder="Şifrenizi girin"
                              />
                              <button
                                  onClick={() => setShowDeleteConfirm(true)}
                                  className="px-4 py-2 bg-red-500 text-white rounded hover:bg-red-600"
                              >
                                  <FontAwesomeIcon icon={faTrash} className="mr-1" /> Hesabı Sil
                              </button>
                          </>
                      ) : (
                          <div className="p-4 border rounded bg-red-50 text-red-700 space-y-3">
                              <p>⚠️ Hesabınızı kalıcı olarak silmek istediğinize emin misiniz?</p>
                              <div className="flex space-x-3">
                                  <button
                                      onClick={handleDeleteAccount}
                                      className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
                                  >
                                      Evet, Sil
                                  </button>
                                  <button
                                      onClick={() => setShowDeleteConfirm(false)}
                                      className="px-4 py-2 bg-gray-300 text-gray-700 rounded hover:bg-gray-400"
                                  >
                                      Vazgeç
                                  </button>
                              </div>
                          </div>
                      )}
                  </div>
              )}

              {/* Avatar */}
              {activeForm === "avatar" && (
                  <div>
                      <h4 className="font-semibold mb-2">Avatar Seç</h4>
                      <div className="flex space-x-4">
                          {avatars.map((src, i) => (
                              <img
                                  key={i}
                                  src={src}
                                  alt={`Avatar ${i}`}
                                  className={`w-16 h-16 rounded-full cursor-pointer border-4 ${avatarIndex === i ? "border-blue-500" : "border-transparent"}`}
                                  onClick={() => setAvatarIndex(i)}
                              />
                          ))}
                      </div>
                      <button
                          onClick={handleUpdateProfile}   // ✅ artık backend’e isteği gönderiyor
                          className="mt-3 px-4 py-2 bg-purple-500 text-white rounded hover:bg-purple-600"
                      >
                          Kaydet
                      </button>
                  </div>
              )}

              {activeForm === "about" && (
                  <div>
                      <textarea
                          className="border p-2 rounded w-full mb-2"
                          rows="3"
                          value={aboutMe}
                          onChange={(e) => setAboutMe(e.target.value)}
                          placeholder="Hakkınızda kısa bir yazı..."
                      />
                      <button
                          onClick={handleUpdateProfile}
                          className="px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600"
                      >
                          <FontAwesomeIcon icon={faSave} className="mr-1" /> Kaydet
                      </button>
                  </div>
              )}

              {/* Inline status message */}
              {statusMessage && (
                  <div
                      className={`mt-4 p-3 rounded ${statusType === "success"
                          ? "bg-green-100 text-green-700"
                          : "bg-red-100 text-red-700"
                          }`}
                  >
                      {statusMessage}
                  </div>
              )}
          </div>

      {/* Tabs */}
      <div className="bg-white rounded-xl shadow-md p-6">
        <div className="flex border-b border-gray-200 mb-4">
          <button
            className={`flex-1 py-2 text-center font-semibold ${
              activeTab === "history"
                ? "border-b-4 border-blue-500 text-blue-500"
                : "text-gray-500 hover:text-blue-500"
            }`}
            onClick={() => setActiveTab("history")}
          >
            Geçmiş Rotalarım
          </button>
          <button
            className={`flex-1 py-2 text-center font-semibold ${
              activeTab === "favorites"
                ? "border-b-4 border-blue-500 text-blue-500"
                : "text-gray-500 hover:text-blue-500"
            }`}
            onClick={() => setActiveTab("favorites")}
          >
            Favorilerim
          </button>
        </div>

        <div>
          {activeTab === "history" && <PastRoutesList />}
          {activeTab === "favorites" && <p>Buraya favori rotaların listesi gelecek...</p>}
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;
