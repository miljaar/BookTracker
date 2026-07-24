import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useCurrentMember } from "./useCurrentMember";
import type { UpdateMemberRequest } from "../members/types";
import { updateMember } from "../members/membersApi";
import { ApiError } from "../api";
import { removeAccessToken } from "./tokenStorage";


function readMemberId(value: string | undefined) {
    const memberId = Number(value);
    return Number.isInteger(memberId) && memberId > 0 ? memberId : null;
}

export function EditAccountPage() {
    const [formError, setFormError] = useState<string | null>(null);
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const memberQuery = useCurrentMember();
    const memberId = memberQuery.data?.id ?? null;

    if (memberId === null) {
        return (
            <main>
                <h1>Invalid member id</h1>
                <Link to="/account">Back to account</Link>
            </main>
        );
    }

    const updateMutation = useMutation({
        mutationFn: (request: UpdateMemberRequest) => {
            return updateMember(memberId, request);
        },
        onSuccess: () => {
            // force user to re-login to prevent working with old data that resides in token
            removeAccessToken();
            queryClient.removeQueries({ queryKey: ["current-member"] });
            navigate("/login");
        }
    });

    function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();
        setFormError(null);

        if (!memberQuery.data) {
            return;
        }

        const formData = new FormData(event.currentTarget);
        const name = formData.get("name")?.toString().trim() ?? "";
        const email = formData.get("email")?.toString().trim() ?? "";

        if (!name || !email) {
            setFormError("Enter name and email");
            return;
        }

        updateMutation.mutate({
            name,
            email
        });
    }

    if (memberQuery.isPending) {
        return <p>Loading member...</p>;
    }

    const queryNotFound = memberQuery.error instanceof ApiError && memberQuery.error.status === 404;

    if (queryNotFound) {
        return (
            <main>
                <p>Member not found</p>
                <Link to="/account">Back to account</Link>
            </main>);
    }

    if (memberQuery.isError) {
        return <p>Could not load member.</p>;
    }

    const member = memberQuery.data;
    const mutationStatus = updateMutation.error instanceof ApiError ? updateMutation.error.status : null;

    if (member.role !== "Administrator" && memberId !== member.id) {
        return (
            <main>
                <p>Only administrators can edit members.</p>
                <Link to="/account">Back to account</Link>
            </main>);
    }

    return (
        <main>
            <Link to={`/account`}>Cancel</Link>
            <h1>Edit {member.name}</h1>

            <form key={member.email} onSubmit={handleSubmit}>
                <label>
                    Name
                    <input
                        name="name"
                        defaultValue={member.name}
                        maxLength={100}
                        required
                    />
                </label>

                <label>
                    Email
                    <input
                        name="email"
                        defaultValue={member.email}
                        maxLength={100}
                        required
                    />
                </label>

                <button type="submit" disabled={updateMutation.isPending}>
                    {updateMutation.isPending ? "Saving..." : "Save changes"}
                </button>
            </form>

            {formError && <p>{formError}</p>}
            {mutationStatus === 400 && <p>The API rejected the member data.</p>}
            {mutationStatus === 401 && <p>Your login is missing or expired.</p>}
            {mutationStatus === 403 && (
                <p>Only administrators can edit members.</p>
            )}
            {mutationStatus === 404 && <p>This member no longer exists.</p>}
            {mutationStatus === 409 && (
                <div>
                    <p>
                        This email is already in use. Your changes where not saved.
                    </p>
                </div>
            )}
            {updateMutation.isError && mutationStatus === null && (
                <p>Could not update member.</p>
            )}
        </main>
    );
}