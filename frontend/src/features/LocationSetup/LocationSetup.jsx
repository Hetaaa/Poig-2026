import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { LocationPicker } from "../../common/components/LocationPicker/LocationPicker";
import { useLocationStore } from "../../common/stores/locationStore";
import "./LocationSetup.scss";

export function LocationSetup() {
  const { saveLocation } = useLocationStore();
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  async function handleSave(lat, lng, cityName) {
    setSaving(true);
    setError("");
    try {
      await saveLocation(lat, lng, cityName);
      navigate("/", { replace: true });
    } catch {
      setError("Nie udało się zapisać lokalizacji. Spróbuj ponownie.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="setup-page">
      <div className="setup-card">
        <aside className="setup-side">
          <div className="setup-brand">Smart Wardrobe</div>
          <div className="setup-side-title">Skąd jesteś?</div>
          <p className="setup-side-text">
            Wyszukaj swoje miasto lub kliknij na mapie, aby ustawić lokalizację.
            Będzie ona używana do pobierania prognozy pogody i dopasowywania
            outfitów do warunków atmosferycznych.
          </p>
          <div className="setup-note">
            Lokalizację możesz zmienić w każdej chwili w ustawieniach.
          </div>
        </aside>

        <section className="setup-content">
          <div className="setup-header">
            <h1>Ustaw swoją lokalizację</h1>
            <p>Potrzebujemy jej, aby pokazać Ci pogodę i dopasować outfity</p>
          </div>

          {error && <div className="setup-error">{error}</div>}

          <LocationPicker onSave={handleSave} saving={saving} />
        </section>
      </div>
    </div>
  );
}
