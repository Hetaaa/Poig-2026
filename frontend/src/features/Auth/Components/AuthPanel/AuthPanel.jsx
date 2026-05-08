export default function AuthPanel({
  title,
  subtitle,
  fields,
  form,
  onChange,
  onSubmit,
  isBusy,
  status,
  formError,
  error,
  statusText,
  submitLabel,
  footerText,
  footerActionText,
  onFooterAction,
}) {
  return (
    <section className="auth-panel">
      <div className="background-decor" />
      <header className="auth-header">
        <h1>{title}</h1>
        <p>{subtitle}</p>
      </header>
      <form className="auth-form" onSubmit={onSubmit}>
        {fields.map((field) => (
          <label className="auth-label" key={field.name}>
            {field.label}
            <input
              className="auth-input"
              type={field.type}
              name={field.name}
              value={form[field.name] || ""}
              onChange={onChange}
              autoComplete={field.autoComplete}
              placeholder={field.placeholder}
              disabled={isBusy}
            />
          </label>
        ))}
        {formError && <div className="auth-error">{formError}</div>}
        {!formError && error && <div className="auth-error">{error}</div>}
        {status === "loading" && (
          <div className="auth-status">{statusText}</div>
        )}
        <button className="auth-button" type="submit" disabled={isBusy}>
          {submitLabel}
        </button>
      </form>
      <div className="auth-footer">
        {footerText}
        <button type="button" className="auth-link" onClick={onFooterAction}>
          {footerActionText}
        </button>
      </div>
    </section>
  );
}
