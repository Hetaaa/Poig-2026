import { useEffect, useState } from "react";
import { useAlertStore } from "./alertStore";
import "./AlertBox.scss";

function AlertBoxInner({ message, type, visible }) {
  const [leaving, setLeaving] = useState(false);

  useEffect(() => {
    if (!visible) setLeaving(true);
  }, [visible]);

  return (
    <div
      className={`alert-box alert-box--${type} ${leaving ? "alert-leave" : "alert-enter"}`}
    >
      {message}
    </div>
  );
}

export function AlertBox() {
  const { message, type, visible, alertKey } = useAlertStore();
  const [show, setShow] = useState(false);

  useEffect(() => {
    if (visible) {
      setShow(true);
    } else {
      const t = setTimeout(() => setShow(false), 400);
      return () => clearTimeout(t);
    }
  }, [visible, alertKey]);

  if (!show) return null;

  return (
    <AlertBoxInner key={alertKey} message={message} type={type} visible={visible} />
  );
}
