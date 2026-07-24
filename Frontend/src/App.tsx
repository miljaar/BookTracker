import { Route, Routes } from "react-router-dom";
import { AccountPage } from "./auth/AccountPage";
import { LoginPage } from "./auth/LoginPage";
import { BookListPage } from "./books/bookListPage";
import { BookDetailsPage } from "./books/bookDetailsPage";
import { RequireAdministrator } from "./auth/requireAdministrator";
import { CreateBookPage } from "./books/CreateBookPage";
import { EditBookPage } from "./books/editBookPage";
import { RegisterPage } from "./members/RegisterPage";
import { Navigation } from "./Navigation";
import { MemberDetailsPage } from "./members/memberDetailsPage";
import { MemberListPage } from "./members/memberListPage";
import { EditAccountPage } from "./auth/editAccountPage";

function HomePage() {
  return <h1>Book Tracker</h1>;
}

export default function App() {
  return (
    <>
      <Navigation />

      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/account" element={<AccountPage />} />
        <Route path="/books" element={<BookListPage />} />
        <Route path="register" element={<RegisterPage />} />
        <Route path="/account/edit" element={< EditAccountPage />} />
        <Route element={<RequireAdministrator />}>
          <Route path="/books/new" element={<CreateBookPage />} />
          <Route path="/books/:bookId/edit" element={<EditBookPage />} />
          <Route path="/members" element={<MemberListPage />} />
          <Route path="/members/:memberId" element={<MemberDetailsPage />} />
        </Route>
        <Route path="/books/:bookId" element={<BookDetailsPage />} />
      </Routes >
    </>
  )
}