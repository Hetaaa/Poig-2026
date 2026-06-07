import { useState, useCallback, useRef } from "react";
import { MapContainer, TileLayer, Marker, useMapEvents } from "react-leaflet";
import { OpenStreetMapProvider } from "leaflet-geosearch";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import "./LocationPicker.scss";

// Fix for default marker icons in Leaflet + bundlers
import markerIconUrl from "leaflet/dist/images/marker-icon.png";
import markerIcon2xUrl from "leaflet/dist/images/marker-icon-2x.png";
import markerShadowUrl from "leaflet/dist/images/marker-shadow.png";

delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconUrl: markerIconUrl,
  iconRetinaUrl: markerIcon2xUrl,
  shadowUrl: markerShadowUrl,
});

const provider = new OpenStreetMapProvider();

function MapClickHandler({ onPick }) {
  useMapEvents({
    click(e) {
      onPick(e.latlng.lat, e.latlng.lng, null);
    },
  });
  return null;
}

export function LocationPicker({
  onSave,
  saving = false,
  initialPosition = null,
  mapHeight = "22rem",
}) {
  const [markerPos, setMarkerPos] = useState(
    initialPosition
      ? { lat: initialPosition.lat, lng: initialPosition.lng }
      : null,
  );
  const [cityName, setCityName] = useState(initialPosition?.cityName ?? "");
  const [query, setQuery] = useState(initialPosition?.cityName ?? "");
  const [suggestions, setSuggestions] = useState([]);
  const [searching, setSearching] = useState(false);
  const mapRef = useRef(null);

  const handlePick = useCallback((lat, lng, name) => {
    setMarkerPos({ lat, lng });
    if (name) setCityName(name);
    setSuggestions([]);
  }, []);

  async function handleSearch(e) {
    const val = e.target.value;
    setQuery(val);
    if (val.trim().length < 3) {
      setSuggestions([]);
      return;
    }
    setSearching(true);
    try {
      const results = await provider.search({ query: val });
      setSuggestions(results.slice(0, 5));
    } catch {
      setSuggestions([]);
    } finally {
      setSearching(false);
    }
  }

  function selectSuggestion(result) {
    const lat = result.y;
    const lng = result.x;
    handlePick(lat, lng, result.label);
    setQuery(result.label);
    setSuggestions([]);
    if (mapRef.current) {
      mapRef.current.setView([lat, lng], 12);
    }
  }

  function handleSave() {
    if (!markerPos) return;
    onSave(markerPos.lat, markerPos.lng, cityName || query);
  }

  return (
    <div className="location-picker">
      <div className="lp-search-wrapper">
        <input
          className="lp-search-input"
          type="text"
          placeholder="Wyszukaj miasto..."
          value={query}
          onChange={handleSearch}
          autoComplete="off"
        />
        {searching && <span className="lp-searching">Szukam...</span>}
        {suggestions.length > 0 && (
          <ul className="lp-suggestions">
            {suggestions.map((r, i) => (
              <li
                key={i}
                className="lp-suggestion-item"
                onClick={() => selectSuggestion(r)}
              >
                {r.label}
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="lp-map-wrapper" style={{ height: mapHeight }}>
        <MapContainer
          center={
            initialPosition
              ? [initialPosition.lat, initialPosition.lng]
              : [52.2297, 21.0122]
          }
          zoom={initialPosition ? 11 : 5}
          className="lp-map"
          ref={mapRef}
        >
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          <MapClickHandler onPick={handlePick} />
          {markerPos && <Marker position={[markerPos.lat, markerPos.lng]} />}
        </MapContainer>
      </div>

      {markerPos && (
        <p className="lp-coords">
          {cityName || query ? (
            <>
              <strong>{cityName || query}</strong> &nbsp;
            </>
          ) : null}
          ({markerPos.lat.toFixed(4)}, {markerPos.lng.toFixed(4)})
        </p>
      )}

      <button
        type="button"
        className="lp-save-btn"
        onClick={handleSave}
        disabled={!markerPos || saving}
      >
        {saving ? "Zapisywanie..." : "Zapisz lokalizację"}
      </button>
    </div>
  );
}
