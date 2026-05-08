export default function AuthSidePanel({ isRegister, onSwitch }) {
  return (
    <aside className="auth-side">
      <div className="auth-brand">Smart Wardrobe</div>
      <div className="auth-side-title">Styl pod pogode, bez zgadywania.</div>
      <p className="auth-side-text">
        Twoj outfit startuje od prognozy. Dzisiaj wygladasz dobrze, jutro tez.
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
      <div className="auth-note">Token zapisywany lokalnie w przegladarce.</div>
    </aside>
  );
}
