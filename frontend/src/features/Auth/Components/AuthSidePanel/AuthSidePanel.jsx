export function AuthSidePanel({ isRegister, onSwitch }) {
  return (
    <aside className="auth-side">
      <div className="auth-brand">Smart Wardrobe</div>
      <div className="auth-side-title">Styl pod pogodę, bez zgadywania.</div>
      <p className="auth-side-text">
        Twój outfit startuje od prognozy. Dzisiaj wyglądasz dobrze, jutro też.
      </p>
      <div className="auth-switch">
        <div className="switch-track">
          <span className="switch-indicator" />
          <button
            type="button"
            className={`switch-button ${!isRegister ? "is-active" : ""}`}
            onClick={() => onSwitch("login")}
          >
            Logowanie
          </button>
          <button
            type="button"
            className={`switch-button ${isRegister ? "is-active" : ""}`}
            onClick={() => onSwitch("register")}
          >
            Rejestracja
          </button>
        </div>
      </div>
      <div className="auth-note">Ubieraj się po swojemu ale z głową w chmurze.</div>
    </aside>
  );
}
