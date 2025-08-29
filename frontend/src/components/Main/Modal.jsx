import React, { useState } from "react";
// RatingAndEmojiPicker bileşenini içeri aktarıyoruz.
import RatingAndEmojiPicker from "./RatingAndEmojiPicker"; 

const Modal = ({ isOpen, onClose, children, onRateSubmit }) => {
    // Hooks'ları koşulsuz olarak en üst seviyede tanımlıyoruz.
    const [currentRating, setCurrentRating] = useState(0);
    const [currentEmoji, setCurrentEmoji] = useState('');
    const [newComment, setNewComment] = useState("");

    // isOpen false ise null döndürüp bileşenin render'ını durduruyoruz.
    if (!isOpen) return null;

    const handleRatingChange = (rating, emoji) => {
        setCurrentRating(rating);
        setCurrentEmoji(emoji);
    };

    const handleSubmit = () => {
        // Puan, emoji ve yorumu tek bir nesne olarak gönderiyoruz
        onRateSubmit({
            rating: currentRating,
            emoji: currentEmoji,
            comment: newComment.trim()
        });
        onClose();
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-2xl shadow-xl w-full max-w-lg p-6 relative overflow-hidden">
                <button
                    className="absolute top-4 right-4 text-gray-500 hover:text-gray-700 text-lg font-bold"
                    onClick={onClose}
                >
                    ✕
                </button>
                {children}

                {/* Yorumlar ve Derecelendirme Formu */}
                <h3 className="text-xl font-semibold text-center mt-6 mb-4">Bu öğeyi değerlendirin!</h3>
                {/* Ayrı bir bileşen olarak kullanıyoruz */}
                <RatingAndEmojiPicker onRate={handleRatingChange} />

                {/* Yorum Ekleme */}
                <div className="mt-6 flex flex-col sm:flex-row gap-2">
                    <input
                        type="text"
                        placeholder="Yorum ekle..."
                        value={newComment}
                        onChange={(e) => setNewComment(e.target.value)}
                        className="border rounded-lg px-3 py-2 flex-1 focus:outline-none focus:ring-2 focus:ring-orange-400"
                    />
                    <button
                        className="px-4 py-2 bg-orange-400 text-white rounded-lg hover:bg-orange-500"
                        onClick={handleSubmit}
                        disabled={currentRating === 0 || currentEmoji === ''}
                    >
                        Gönder
                    </button>
                </div>

            </div>
        </div>
    );
};

export default Modal;
