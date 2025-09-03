import React, { useState } from "react";
import { createTravelRoute } from "../../services/TravelRouteService";

export default function CreateTravelRouteButton({
  startPoiId,
  endPoiId,
  poiIds,
  disabledAccess,
  maxWalkingDistanceMeters,
  children = "TravelRoute Oluştur",
  onSuccess,
  onError,
}) {
  const [loading, setLoading] = useState(false);

  const handleClick = async () => {
    try {
      setLoading(true);
      const res = await createTravelRoute({
        startPoiId,
        endPoiId,
        poiIds,
        disabledAccess,
        maxWalkingDistanceMeters,
      });
      onSuccess?.(res);
    } catch (err) {
      console.error(err);
      onError?.(err);
      alert("TravelRoute oluşturulamadı");
    } finally {
      setLoading(false);
    }
  };

  return (
    <button onClick={handleClick} disabled={loading}>
      {loading ? "Kaydediliyor..." : children}
    </button>
  );
}