import React, { useState } from 'react';
import { Dropdown } from 'primereact/dropdown';
import { InputNumber } from 'primereact/inputnumber';

const FilterSort = ({ onFilterSortChange }) => {
  const [sortBy, setSortBy] = useState('popularity');
  const [sortOrder, setSortOrder] = useState('desc');
  const [minRating, setMinRating] = useState(null);
  const [minComments, setMinComments] = useState(null);

  const sortOptions = [
    { label: 'En Popüler', value: 'popularity' },
    { label: 'En Yüksek Puan', value: 'rating' },
    { label: 'En Çok Yorum', value: 'comments' },
    { label: 'En Yeni', value: 'newest' },
    { label: 'En Eski', value: 'oldest' },
  ];

  const handleApplyFilters = React.useCallback(() => {
    onFilterSortChange({ sortBy, sortOrder, minRating: minRating || undefined, minComments: minComments || undefined });
  }, [sortBy, sortOrder, minRating, minComments, onFilterSortChange]);

  React.useEffect(() => {
    handleApplyFilters();
  }, [handleApplyFilters]);

  return (
    <div className="p-fluid">
      <div className="field mb-4">
        <label htmlFor="sortBy" className="block text-900 font-medium mb-2">Sıralama Ölçütü</label>
        <Dropdown
          id="sortBy"
          value={sortBy}
          options={sortOptions}
          onChange={(e) => setSortBy(e.value)}
          placeholder="Sırala"
          className="w-full"
        />
      </div>

      <div className="field mb-4">
        <label htmlFor="minRating" className="block text-900 font-medium mb-2">Minimum Puan</label>
        <InputNumber
          id="minRating"
          value={minRating}
          onValueChange={(e) => setMinRating(e.value)}
          mode="decimal"
          min={0}
          max={5}
          step={0.5}
          showButtons
          suffix=" yıldız"
          className="w-full"
        />
      </div>

      <div className="field">
        <label htmlFor="minComments" className="block text-900 font-medium mb-2">Minimum Yorum Sayısı</label>
        <InputNumber
          id="minComments"
          value={minComments}
          onValueChange={(e) => setMinComments(e.value)}
          mode="decimal"
          min={0}
          showButtons
          className="w-full"
        />
      </div>
    </div>
  );
};

export default FilterSort;