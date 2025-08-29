import React, { useState } from "react";

const RatingAndEmojiPicker = ({ onRate, initialRating = 0, initialEmoji = '' }) => {
    const [hoverRating, setHoverRating] = useState(0);
    const [selectedRating, setSelectedRating] = useState(initialRating);
    const [selectedEmoji, setSelectedEmoji] = useState(initialEmoji);

    const emojis = ['😊', '😐', '😞', '🥳', '🤯'];

    const handleStarClick = (index) => {
        setSelectedRating(index);
        onRate(index, selectedEmoji);
    };

    const handleEmojiClick = (emoji) => {
        setSelectedEmoji(emoji);
        onRate(selectedRating, emoji);
    };

    return (
        <div className="flex flex-col items-center mt-4">
            {/* Yıldız Derecelendirme */}
            <div className="flex mb-4">
                {[1, 2, 3, 4, 5].map((starIndex) => (
                    <svg
                        key={starIndex}
                        className={`w-8 h-8 cursor-pointer transition-colors duration-200 
                                    ${(hoverRating || selectedRating) >= starIndex ? 'text-yellow-400' : 'text-gray-300'}`}
                        fill="currentColor"
                        viewBox="0 0 20 20"
                        onMouseEnter={() => setHoverRating(starIndex)}
                        onMouseLeave={() => setHoverRating(0)}
                        onClick={() => handleStarClick(starIndex)}
                    >
                        <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.538 1.118l-2.8-2.034a1 1 0 00-1.176 0l-2.8 2.034c-.783.57-1.838-.197-1.538-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.381-1.81.588-1.81h3.462a1 1 0 00.95-.69l1.07-3.292z" />
                    </svg>
                ))}
            </div>

            {/* Emoji Seçimi */}
            <div className="flex gap-2">
                {emojis.map((emoji, index) => (
                    <span
                        key={index}
                        className={`text-3xl cursor-pointer transition-transform duration-200 hover:scale-110
                                    ${selectedEmoji === emoji ? 'border-2 border-blue-500 rounded-full p-1' : ''}`}
                        onClick={() => handleEmojiClick(emoji)}
                    >
                        {emoji}
                    </span>
                ))}
            </div>

            {selectedRating > 0 && selectedEmoji && (
                <p className="mt-4 text-lg text-gray-700">
                    Seçiminiz: {selectedRating} yıldız ve {selectedEmoji}
                </p>
            )}
        </div>
    );
};

export default RatingAndEmojiPicker;