import React, { useState } from "react";
import { FaStar, FaUsers, FaMapMarkerAlt } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import Modal from "./Modal";

// API Base URL (dotenv’den geliyor)
const API_URL = process.env.REACT_APP_API_URL;

const categoryImages = {
  "Yemek": `${API_URL.replace("/api", "")}/images/categories/yemek.png`,
  "Alışveriş": `${API_URL.replace("/api", "")}/images/categories/alisveris.png`,
  "Eğlence": `${API_URL.replace("/api", "")}/images/categories/eglence.png`,
  "Kültür": `${API_URL.replace("/api", "")}/images/categories/kultur.png`,
  "OnemliNoktalar": `${API_URL.replace("/api", "")}/images/categories/onemli.png`,
  "Karayolu": `${API_URL.replace("/api", "")}/images/categories/otopark.png`,
  "TarihiTuristikTesisler": `${API_URL.replace("/api", "")}/images/categories/tarihi.png`,
  "KültürelTesisler": `${API_URL.replace("/api", "")}/images/categories/kultur.png`,
};

const MainCard = ({
  id,
  imageUrl,
  name,
  location,
  description,
  tags,
  visitors,
  category,
  rating,
  comments,
  latitude,
  longitude,
}) => {
  const navigate = useNavigate();
  const defaultImage = "/anasayfa.png";
  const resolvedImage = imageUrl || categoryImages[category] || defaultImage;

  const [allSubmissions, setAllSubmissions] = useState(
    (comments || []).map((r) => ({
      type: "full",
      text: r.comment,
      rating: r.rating,
      emoji: r.emoji,
      createdAt: r.createdAt,
    }))
  );
  const [isModalOpen, setIsModalOpen] = useState(false);

  // --- Backend’den yorumları çek ---
  const loadReviews = async () => {
    if (!id) return;
    try {
      const res = await fetch(`${API_URL}/pois/${id}/reviews`);
      if (!res.ok) {
        console.error("Yorumlar yüklenemedi:", res.statusText);
        return;
      }
      const data = await res.json();
      setAllSubmissions(
        (data || []).map((r) => ({
          type: "full",
          text: r.comment,
          rating: r.rating,
          emoji: r.emoji,
          createdAt: r.createdAt,
        }))
      );
    } catch (err) {
      console.error("Yorum çekme hatası:", err);
    }
  };

  // --- Modal açıldığında yorumları getir ---
  const openModal = () => {
    setIsModalOpen(true);
    loadReviews();
  };
  const closeModal = () => setIsModalOpen(false);

  // --- Yeni yorum gönder ---
  const handleSubmission = async (submission) => {
    try {
      const res = await fetch(`${API_URL}/pois/${id}/reviews`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(submission),
      });

      if (!res.ok) {
        throw new Error(await res.text());
      }

      const newReview = await res.json();

      if (newReview.comment && newReview.comment.length > 0) {
        setAllSubmissions((prev) => [
          {
            type: "full",
            text: newReview.comment,
            rating: newReview.rating,
            emoji: newReview.emoji,
            createdAt: newReview.createdAt,
          },
          ...prev,
        ]);
      } else {
        setAllSubmissions((prev) => [
          {
            type: "ratingOnly",
            rating: newReview.rating,
            emoji: newReview.emoji,
            createdAt: newReview.createdAt,
          },
          ...prev,
        ]);
      }

      closeModal();
    } catch (err) {
      console.error("Yorum gönderme hatası:", err);
      alert("Yorum gönderilemedi: " + err.message);
    }
  };

  // POI’yi rotaya ekle
  const handleAddToRoute = () => {
    navigate("/map", {
      state: {
        poiToAdd: { id, name, latitude, longitude, category },
      },
    });
  };

  return (
    <div className="bg-white rounded-lg shadow-md overflow-hidden relative mb-20">
      <img
        src={resolvedImage}
        alt={name || "POI Resmi"}
        className="w-full h-48 object-cover"
      />

      <div className="absolute top-2 left-2 bg-gray-900 text-white text-xs px-2 py-1 rounded flex items-center space-x-1">
        <FaStar className="text-yellow-400" />
        <span>{rating}</span>
      </div>

      <div className="absolute top-2 right-2 bg-gray-200 text-gray-700 text-xs px-2 py-1 rounded">
        {category}
      </div>

      <div className="p-4">
        <h3 className="font-bold text-lg">{name}</h3>
        <p className="text-sm text-gray-500 flex items-center">
          <FaMapMarkerAlt className="mr-1" /> {location}
        </p>
        <p className="text-sm text-gray-600 mt-2">{description}</p>

        <div className="flex flex-wrap gap-2 mt-3">
          {tags?.map((tag, i) => (
            <span
              key={i}
              className="bg-gray-100 text-gray-700 px-2 py-1 text-xs rounded-full"
            >
              {tag}
            </span>
          ))}
        </div>

        <div className="flex justify-between items-center mt-4 text-sm text-gray-500">
          <div className="flex items-center space-x-1">
            <FaUsers />
            <span>{visitors} ziyaretçi</span>
          </div>
          <div className="flex space-x-2">
            <button
              className="px-3 py-1 border rounded-lg hover:bg-gray-100"
              onClick={openModal}
            >
              Detaylar ve Yorumlar
            </button>
            <button
              className="px-3 py-1 bg-orange-400 text-white rounded-lg hover:bg-orange-500"
              onClick={handleAddToRoute}
            >
              Rotaya Ekle
            </button>
          </div>
        </div>
      </div>

      {/* Modal */}
      <Modal
        isOpen={isModalOpen}
        onClose={closeModal}
        onRateSubmit={handleSubmission}
        poiId={id}
      >
        <h2 className="text-2xl font-bold mb-2">{name}</h2>
        <p className="text-gray-600 mb-4">{description}</p>

        <h3 className="font-semibold mb-2">Yorumlar:</h3>
        <div className="max-h-64 overflow-y-auto mb-4">
          {allSubmissions.length > 0 ? (
            allSubmissions.map((submission, i) => (
              <p
                key={i}
                className="text-sm text-gray-700 mb-2 border-b pb-1"
              >
                {submission.type === "full"
                  ? `Değerlendirme: ${submission.rating} yıldız ${submission.emoji} - ${submission.text}`
                  : submission.type === "ratingOnly"
                  ? `Değerlendirme: ${submission.rating} yıldız ve ${submission.emoji}`
                  : submission.type === "comment"
                  ? submission.text
                  : null}
              </p>
            ))
          ) : (
            <p className="text-sm text-gray-500">Henüz yorum yok.</p>
          )}
        </div>
      </Modal>
    </div>
  );
};

export default MainCard;
