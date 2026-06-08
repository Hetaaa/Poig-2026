import { Navbar } from "./features/Navbar/Navbar";
import { Sidebar } from "./features/Sidebar/Sidebar";
import { Outlet } from "react-router-dom";
import { useAddElementStore } from "./features/AddClothing/addElementStore";
import { AlertBox } from "./common/components/AlertBox/AlertBox";

import "./App.scss";
import { AddElement } from "./features/AddClothing/Components/AddElement";

export default function App() {
  const {showAdd, closeAdd} = useAddElementStore();

  return (
    <>
      <Navbar />
      <div className="app-content">
        <Sidebar />
        <main className="app-main">
          <Outlet/>
        </main>
      </div>

      {showAdd && <AddElement onClose={closeAdd}/>}
      <AlertBox />
    </>
  );
}
