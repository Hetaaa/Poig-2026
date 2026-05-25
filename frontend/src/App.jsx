import { Navbar } from "./features/Navbar/Navbar";
import { Sidebar } from "./features/Sidebar/Sidebar";
import { Outlet } from "react-router-dom";
import Greetings from "./features/Greetings/Greetings";
import Weather from "./features/Weather/Weather";
import Outfit from "./features/Outfit/Outfit";
import PlannerTitle from "./features/PlannerTitle/PlannerTitle"
import OutfitPlanner from "./features/OutfitPlanner/OutfitPlanner";
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
          <Greetings />
          <Weather/>
          <Outfit/>
          <PlannerTitle/>
          <OutfitPlanner/>
        </main>
      </div>
    </>
  );
}
