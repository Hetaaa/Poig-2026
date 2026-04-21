import {
  AiOutlineHome,
  AiOutlineHeart,
  AiOutlinePlus,
  AiOutlineSetting,
} from "react-icons/ai";
import { NavLink } from "react-router-dom";
import { CgShoppingBag } from "react-icons/cg";
import { MdLogout } from "react-icons/md";
import "./Sidebar.scss";

export default function Sidebar() {
  function handleLogout() {
    // TODO: Implement logout logic here
    console.log("User logged out");
  }

  return (
    <div className="sidebar">
      <nav>
        <NavLink to="/" end className="sidebar-element">
          <AiOutlineHome className="sidebar-element-icon" />
          Strona główna
        </NavLink>
        <NavLink to="/favourites" className="sidebar-element">
          <AiOutlineHeart className="sidebar-element-icon" />
          Ulubione
        </NavLink>
        <NavLink to="/wardrobe" className="sidebar-element">
          <CgShoppingBag className="sidebar-element-icon" />
          Garderoba
        </NavLink>
        <NavLink to="/add-clothing" className="sidebar-element">
          <AiOutlinePlus className="sidebar-element-icon" />
          Dodaj ubranie
        </NavLink>
      </nav>

      <div className="sidebar-bottom">
        <NavLink to="/settings" className="sidebar-element">
          <AiOutlineSetting className="sidebar-element-icon" />
          Ustawienia
        </NavLink>
        <button className="sidebar-element" onClick={handleLogout}>
          <MdLogout className="sidebar-element-icon" />
          Wyloguj się
        </button>
      </div>
    </div>
  );
}
