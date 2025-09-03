import React, { useState } from "react";
import { FaStar, FaUsers, FaMapMarkerAlt } from "react-icons/fa";
import Modal from "./Modal";

// Kategoriye göre resim fallback eşleştirmesi (backend URL ile)
const categoryImages = {
  "Yemek": "http://localhost:5136/images/categories/yemek.png",
  "Alışveriş": "http://localhost:5136/images/categories/alisveris.png",
  "Eğlence": "http://localhost:5136/images/categories/eglence.png",
  "Kültür": "http://localhost:5136/images/categories/kultur.png",
  "OnemliNoktalar": "http://localhost:5136/images/categories/onemli.png",
  "Karayolu": "http://localhost:5136/images/categories/otopark.png",
  "TarihiTuristikTesisler": "http://localhost:5136/images/categories/tarihi.png",
  "KültürelTesisler": "http://localhost:5136/images/categories/kulturel.png",
};


const MainCard = ({
  imageUrl,
  name,
  location,
  description,
  tags,
  visitors,
  category,
  rating,
  comments,
}) => {
  // Varsayılan resim
  const defaultImage = "/anasayfa.png";

  // Gösterilecek resim: Öncelik → imageUrl → kategori fallback → default
  const resolvedImage =
    imageUrl || categoryImages[category] || defaultImage;

  // Yorumları state'e al
  const [allSubmissions, setAllSubmissions] = useState(
    (comments || []).map((comment) => ({ type: "comment", text: comment }))
  );
  const [isModalOpen, setIsModalOpen] = useState(false);

  const openModal = () => setIsModalOpen(true);
  const closeModal = () => setIsModalOpen(false);

  const handleSubmission = (submission) => {
    if (submission.comment.length > 0) {
      setAllSubmissions((prevSubmissions) => [
        {
          type: "full",
          text: submission.comment,
          rating: submission.rating,
          emoji: submission.emoji,
        },
        ...prevSubmissions,
      ]);
    } else {
      setAllSubmissions((prevSubmissions) => [
        {
          type: "ratingOnly",
          rating: submission.rating,
          emoji: submission.emoji,
        },
        ...prevSubmissions,
      ]);
    }
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

        {/* Tags */}
        <div className="flex flex-wrap gap-2 mt-3">
          {tags?.map((tag, index) => (
            <span
              key={index}
              className="bg-gray-100 text-gray-700 px-2 py-1 text-xs rounded-full"
            >
              {tag}
            </span>
          ))}
        </div>

        {/* Alt butonlar */}
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
              Detaylar
            </button>
            <button className="px-3 py-1 bg-orange-400 text-white rounded-lg hover:bg-orange-500">
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
