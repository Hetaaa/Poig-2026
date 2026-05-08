import { CgShoppingBag } from "react-icons/cg";
import { AiOutlineUser } from "react-icons/ai";
import { useAuthStore } from "../Auth/authStore";

import "./Navbar.scss";

export default function Navbar() {
  const { username } = useAuthStore();

  return (
    <div className="navbar">
      <aside className="left">
        <div className="logo">
          <CgShoppingBag className="logo-icon" />
        </div>
        <span>Smart Wardrobe - Projekt na POIG</span>
      </aside>
      <aside className="right">
        <span>{username || "Użytkownik"}</span>
        <div className="user-avatar">
          <AiOutlineUser
            style={{ strokeWidth: 30 }}
            className="user-avatar-icon"
          />
        </div>
      </aside>
    </div>
  );
}
