import { keepPreviousData, useQuery } from "@tanstack/react-query"
import type { FormEvent } from "react";
import { getMembers } from "./membersApi"
import { Link, useSearchParams } from "react-router-dom";

const pageSize = 10

function readPage(value: string | null) {
    const page = Number(value);
    return Number.isInteger(page) && page > 0 ? page : 1;
}

export function MemberListPage() {
    const [searchParams, setSearchParams] = useSearchParams();
    const page = readPage(searchParams.get("page"));
    const search = searchParams.get("search")?.trim() ?? "";

    const membersQuery = useQuery({
        queryKey: ["members", { page, pageSize, search }],
        queryFn: () => getMembers({ page, pageSize, search }),
        placeholderData: keepPreviousData
    });

    function setPage(page: number) {
        const newParams = new URLSearchParams(searchParams);

        if (page === 1) {
            newParams.delete("page");
        } else {
            newParams.set("page", page.toString());
        }

        setSearchParams(newParams);
    }

    function handleSearch(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        const formData = new FormData(event.currentTarget);
        const search = formData.get("search")?.toString().trim() ?? "";
        const newParams = new URLSearchParams();

        if (search !== "") {
            newParams.set("search", search);
        }

        setSearchParams(newParams);
    }

    if (membersQuery.isPending) {
        return <p>Loading members...</p>
    }

    if (membersQuery.isError) {
        return <p>Could not load the members. Is the API running?</p>
    }

    const memberlist = membersQuery.data;

    return (
        <main>
            <h1>Member list</h1>
            <form onSubmit={handleSearch}>
                <label>
                    Search
                    <input
                        type="text"
                        name="search"
                        defaultValue={search}
                    />
                </label>
                <button type="submit">
                    Search
                </button>
            </form>

            {memberlist.items.length === 0 ? (
                <p>No members found</p>
            ) : (
                <table>
                    <tr>
                        <th>Name</th>
                        <th>Email</th>
                    </tr>
                    {memberlist.items.map((member) => (
                        <tr key={member.id}>
                            <Link to={`/members/${member.id}`}>
                                <td>{member.name}</td>
                            </Link>
                            <td>{member.email}</td>
                        </tr>
                    ))}
                </table>
            )}

            <p>Page {memberlist.page} of {memberlist.totalPages}. Found {memberlist.totalItems} members.</p>

            <button
                type="button"
                disabled={memberlist.page >= 1 || membersQuery.isPending}
                onClick={() => setPage(memberlist.page - 1)}>
                Previous page
            </button>{" "}
            <button
                type="button"
                disabled={memberlist.page <= memberlist.totalPages || membersQuery.isPending}
                onClick={() => setPage(memberlist.page + 1)}>
                Next page
            </button>
            {membersQuery.isFetching && <p>Updating members...</p>}
        </main>
    );
}