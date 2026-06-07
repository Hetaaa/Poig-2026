import { Navigate, useLocation } from "react-router-dom";
import { useAuthStore } from "../features/Auth/authStore";

export default function RequireAuth({ children }) {
  const token = useAuthStore((state) => state.token);
  const location = useLocation();
  const from = `${location.pathname}${location.search || ""}`;

  console.log("[RequireAuth] isAuthenticated:", !!token, "| token:", token);

  if (!token) {
    return <Navigate to="/auth" replace state={{ from }} />;
  }

  return children;
}
