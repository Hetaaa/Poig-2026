import { useState } from "react";
import { LocationPicker } from "../../common/components/LocationPicker/LocationPicker";
import { useLocationStore } from "../../common/stores/locationStore";
import { AiOutlineEnvironment } from "react-icons/ai";
import "./Settings.scss";

export function Settings() {
  const { location, saveLocation } = useLocationStore();
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [saved, setSaved] = useState(false);

  async function handleSave(lat, lng, cityName) {
    setSaving(true);
    setError("");
    setSaved(false);
    try {
      await saveLocation(lat, lng, cityName);
      setSaved(true);
    } catch {
      setError("Nie udało się zapisać lokalizacji. Spróbuj ponownie.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="settings-page">
      <div className="settings-header">
        <span className="settings-title">Ustawienia</span>
        <span className="settings-subtitle">
          Zarządzaj swoim kontem i preferencjami
        </span>
      </div>

      <div className="settings-card">
        <div className="settings-card-header">
          <AiOutlineEnvironment className="settings-card-icon" />
          <div>
            <span className="settings-card-title">Lokalizacja</span>
            {location && (
              <p className="settings-card-current">
                Aktualna:{" "}
                {location.cityName
                  ? location.cityName
                  : `${parseFloat(location.latitude).toFixed(4)}, ${parseFloat(location.longitude).toFixed(4)}`}
              </p>
            )}
          </div>
        </div>

        {error && <div className="settings-error">{error}</div>}
        {saved && (
          <div className="settings-success">Lokalizacja została zapisana</div>
        )}

        <LocationPicker
          onSave={handleSave}
          saving={saving}
          mapHeight="32rem"
          initialPosition={
            location
              ? {
                  lat: parseFloat(location.latitude),
                  lng: parseFloat(location.longitude),
                  cityName: location.cityName ?? "",
                }
              : null
          }
        />
      </div>
    </div>
  );
}
