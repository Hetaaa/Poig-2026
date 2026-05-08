import Navbar from "./features/Navbar/Navbar";
import Sidebar from "./features/Sidebar/Sidebar";
import { Outlet } from "react-router-dom";
import Greetings from "./features/Greetings/Greetings";
import Weather from "./features/Weather/Weather";
import "./App.scss";

export default function App() {
  return (
    <>
      <Navbar />
      <div className="app-content">
        <Sidebar />
        <main className="app-main">
          <Greetings />
          <Weather />
        </main>
      </div>
    </>
  );
}
