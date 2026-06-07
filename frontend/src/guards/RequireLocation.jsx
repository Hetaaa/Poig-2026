import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import { useLocationStore } from "../common/stores/locationStore";

export default function RequireLocation({ children }) {
  const { location, status, fetchLastLocation } = useLocationStore();
  const [checked, setChecked] = useState(false);

  useEffect(() => {
    fetchLastLocation().finally(() => setChecked(true));
  }, [fetchLastLocation]);

  if (!checked || status === "loading") {
    return null; // czekamy na odpowiedź backendu
  }

  if (status === "not-set" || location === null) {
    return <Navigate to="/location-setup" replace />;
  }

  return children;
}
