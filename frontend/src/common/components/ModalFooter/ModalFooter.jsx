import { AiOutlineClose } from "react-icons/ai";
import { FiSave } from "react-icons/fi";
import "./ModalFooter.scss";

export function ModalFooter({ onClose, onSave, disabled = false }) {
  return (
    <div className="form-footer">
      <button type="button" className="footer-cancel" onClick={onClose}>
        <AiOutlineClose className="micro-icon" />
        <span className="cancel-text">Anuluj</span>
      </button>

      <button
        type="button"
        className="footer-save"
        onClick={onSave}
        disabled={!onSave || disabled}
      >
        <span className="footer-text">Zapisz</span>
        <FiSave className="micro-icon footer-color" />
      </button>
    </div>
  );
}
