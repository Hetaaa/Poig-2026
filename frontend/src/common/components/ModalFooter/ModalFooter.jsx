import { AiOutlineClose } from "react-icons/ai";
import { FiSave } from "react-icons/fi";
import "./ModalFooter.scss"

export function ModalFooter({ onClose, onSave }) {
  return (
    <div className="form-footer">
      <button className="footer-cancel" onClick={onClose}>
        <AiOutlineClose className="micro-icon" />
        <span className="cancel-text">Anuluj</span>
      </button>

      <button className="footer-save" onClick={onSave}>
        <span className="footer-text">Zapisz</span>
        <FiSave className="micro-icon footer-color" />
      </button>
    </div>
  );
}