import {
    useState, type FormEvent
} from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate, useParams } from "react-router-dom";
import { ApiError } from "../api";
import { getBook, updateBook } from "./booksApi";
import type { UpdateBookRequest } from "./types";

function readBookId(value: string | undefined) {
    const bookId = Number(value);
    return Number.isInteger(bookId) && bookId > 0 ? bookId : null;
}

export function EditBookPage() {
    const { bookId: bookIdParameter } = useParams();
    const bookId = readBookId(bookIdParameter);
    const [formError, setFormError] = useState<string | null>(null);
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const bookQuery = useQuery({
        queryKey: ["books", "detail", bookId],
        queryFn: () => {
            if (bookId === null) {
                throw new Error("Invalid book id");
            }

            return getBook(bookId);
        },
        enabled: bookId !== null,
        retry: false,
    });

    const updateMutation = useMutation({
        mutationFn: (request: UpdateBookRequest) => {
            if (bookId === null) {
                throw new Error("Invalid book id");
            }

            return updateBook(bookId, request);
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ["books"] });
            navigate(`/books/${bookId}`);
        },
    });

    function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        setFormError(null);

        if (!bookQuery.data) {
            return;
        }

        const formData = new FormData(event.currentTarget);
        const title = formData.get("title")?.toString().trim() ?? "";
        const author = formData.get("author")?.toString().trim() ?? "";
        const yearValue = formData.get("year")?.toString().trim() ?? "";
        const year = Number(yearValue);

        if (!title || !author || !yearValue || !Number.isInteger(year)) {
            setFormError("Enter a title, author and valid year.");
            return;
        }

        updateMutation.mutate({
            title,
            author,
            year,
            version: bookQuery.data.version
        });
    }

    async function reloadLatest() {
        updateMutation.reset();
        await bookQuery.refetch();
    }

    if (bookId === null) {
        return (
            <main>
                <h1>Invalid book id</h1>
                <Link to="/books">Back to books</Link>
            </main>
        );
    }

    if (bookQuery.isPending) {
        return <p>Loading book...</p>;
    }

    const queryNotFound = bookQuery.error instanceof ApiError && bookQuery.error.status === 404;

    if (queryNotFound) {
        return (
            <main>
                <p>Book not found</p>
                <Link to="/books">Back to books</Link>
            </main>);
    }

    if (bookQuery.isError) {
        return <p>Could not load the book.</p>;
    }

    const book = bookQuery.data;
    const mutationStatus = updateMutation.error instanceof ApiError ? updateMutation.error.status : null;

    return (
        <main>
            <Link to={`/books/${book.id}`}>Cancel</Link>
            <h1>Edit {book.title}</h1>

            <form key={book.version} onSubmit={handleSubmit}>
                <label>
                    Title
                    <input
                        name="title"
                        defaultValue={book.title}
                        maxLength={100}
                        required
                    />
                </label>

                <label>
                    Author
                    <input
                        name="author"
                        defaultValue={book.author}
                        maxLength={100}
                        required
                    />
                </label>

                <label>
                    Year
                    <input
                        name="year"
                        type="number"
                        defaultValue={book.year}
                        required
                    />
                </label>

                <button type="submit" disabled={updateMutation.isPending}>
                    {updateMutation.isPending ? "Saving..." : "Save changes"}
                </button>
            </form>

            {formError && <p>{formError}</p>}
            {mutationStatus === 400 && <p>The API rejected the book data.</p>}
            {mutationStatus === 401 && <p>Your login is missing or expired.</p>}
            {mutationStatus === 403 && (
                <p>Only administrators can edit books.</p>
            )}
            {mutationStatus === 404 && <p>This book no longer exists.</p>}
            {mutationStatus === 409 && (
                <div>
                    <p>
                        This book was changed by another user. Your changes where not saved.
                    </p>
                    <button type="button" onClick={reloadLatest}>
                        Load latest version
                    </button>
                </div>
            )}
            {updateMutation.isError && mutationStatus === null && (
                <p>Could not update the book.</p>
            )}
        </main>
    );
}

