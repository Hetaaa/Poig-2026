import React from "react";
import App from "./App";
import { createRoot } from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import { AuthPage } from "./features/Auth/AuthPage";
import RequireAuth from "./guards/RequireAuth";
import "./main.scss";

import { MainPage } from "./features/MainPage/MainPage";
import { Favourites } from "./features/Favourites/Favourites";
import { Wardrobe} from "./features/Wardrobe/Wardrobe"

// TODO: Replace with actual pages when they are created
function PlaceholderPage({ title }) {
  return <h2>{title}</h2>;
}

const router = createBrowserRouter([
  {
    path: "/auth",
    element: <AuthPage />,
  },
  {
    path: "/",
    element: (
      <RequireAuth>
        <App />
      </RequireAuth>
    ),
    children: [
      {
        index: true,
        element: <MainPage />,
      },
      {
        path: "main-page",
        element: <MainPage />,
      },
      {
        path: "favourites",
        element: <Favourites/>,
      },
      {
        path: "wardrobe",
        element: <Wardrobe />,
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
