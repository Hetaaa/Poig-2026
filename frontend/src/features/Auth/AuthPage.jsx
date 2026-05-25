import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useAuthStore } from "./authStore";
import { AuthPanel } from "./Components/AuthPanel/AuthPanel";
import { AuthSidePanel } from "./Components/AuthSidePanel/AuthSidePanel";
import "./AuthPage.scss";

const initialForm = {
  username: "",
  password: "",
  confirmPassword: "",
};

const loginFields = [
  {
    label: "Login",
    name: "username",
    type: "text",
    autoComplete: "username",
    placeholder: "Wpisz login...",
  },
  {
    label: "Haslo",
    name: "password",
    type: "password",
    autoComplete: "current-password",
    placeholder: "Wpisz haslo...",
  },
];

const registerFields = [
  {
    label: "Login",
    name: "username",
    type: "text",
    autoComplete: "username",
    placeholder: "Wpisz login...",
  },
  {
    label: "Haslo",
    name: "password",
    type: "password",
    autoComplete: "new-password",
    placeholder: "Minimum 6 znakow...",
  },
  {
    label: "Powtorz haslo",
    name: "confirmPassword",
    type: "password",
    autoComplete: "new-password",
    placeholder: "Powtorz haslo...",
  },
];

export function AuthPage() {
  const [mode, setMode] = useState("login");
  const [form, setForm] = useState(initialForm);
  const [formError, setFormError] = useState("");
  const { token, status, error, login, register, clearError } = useAuthStore();

  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from || "/";

  const isRegister = mode === "register";
  const isBusy = status === "loading";

  useEffect(() => {
    if (token) {
      navigate(from, { replace: true });
    }
  }, [from, navigate, token]);

  const handleSwitch = (nextMode) => {
    if (nextMode === mode) return;
    setMode(nextMode);
    setFormError("");
    clearError();
  };

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((prev) => ({ ...prev, [name]: value }));
    if (formError) setFormError("");
    if (error) clearError();
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    if (isBusy) return;

    const username = form.username.trim();
    if (!username || !form.password) {
      setFormError("Wypełnij wszystkie pola");
      return;
    }

    if (isRegister && form.password !== form.confirmPassword) {
      setFormError("Hasła się nie zgadzają");
      return;
    }
    setFormError("");

    try {
      if (isRegister) {
        await register(username, form.password);
      } else {
        await login(username, form.password);
      }
      navigate(from, { replace: true });
    } catch (err) {
      // store handles error state
    }
  };

  return (
    <div className="auth-page">
      <div className={`auth-card ${isRegister ? "is-register" : "is-login"}`}>
        <div className="auth-forms">
          <div className="auth-panels">
            <AuthPanel
              title="Logowanie"
              subtitle="Wroc do swojej szafy i sprawdz pogode."
              fields={loginFields}
              form={form}
              onChange={handleChange}
              onSubmit={handleSubmit}
              isBusy={isBusy}
              status={status}
              formError={formError}
              error={error}
              statusText="Logowanie..."
              submitLabel="Zaloguj sie"
              footerText="Nie masz konta?"
              footerActionText="Zarejestruj sie"
              onFooterAction={() => handleSwitch("register")}
            />
            <AuthPanel
              title="Rejestracja"
              subtitle="Stworz konto i zacznij stylizowac pod pogode."
              fields={registerFields}
              form={form}
              onChange={handleChange}
              onSubmit={handleSubmit}
              isBusy={isBusy}
              status={status}
              formError={formError}
              error={error}
              statusText="Rejestracja..."
              submitLabel="Utworz konto"
              footerText="Masz konto?"
              footerActionText="Zaloguj sie"
              onFooterAction={() => handleSwitch("login")}
            />
          </div>
        </div>
        <AuthSidePanel isRegister={isRegister} onSwitch={handleSwitch} />
      </div>
    </div>
  );
}
