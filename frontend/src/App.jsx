import { Navbar } from "./features/Navbar/Navbar";
import { Sidebar } from "./features/Sidebar/Sidebar";
import { Outlet } from "react-router-dom";
import "./App.scss";
import "@fontsource/lato/400.css"; // Regular
import "@fontsource/lato/700.css";

export default function App() {
  return (
    <>
      <Navbar />
      <div className="app-content">
        <Sidebar />
        <main className="app-main">
          <Outlet/>
        </main>
      </div>
    </>
  );
}
