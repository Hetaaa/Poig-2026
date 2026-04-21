import React from "react";
import App from "./App";
import { createRoot } from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import Testpage from "./features/Test/Testpage";
import "./main.scss";

// TODO: Replace with actual pages when they are created
function PlaceholderPage({ title }) {
  return <h2>{title}</h2>;
}

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      {
        index: true,
        element: <Testpage />,
      },
      {
        path: "main-page",
        element: <Testpage />,
      },
      {
        path: "favourites",
        element: <PlaceholderPage title="Ulubione" />,
      },
      {
        path: "wardrobe",
        element: <PlaceholderPage title="Garderoba" />,
      },
      {
        path: "add-clothing",
        element: <PlaceholderPage title="Dodaj ubranie" />,
      },
      {
        path: "settings",
        element: <PlaceholderPage title="Ustawienia" />,
      },
    ],
  },
]);

createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>,
);
