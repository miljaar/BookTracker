import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useState, type FormEvent } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getMember, updateMember } from "./membersApi";
import type { UpdateMemberRequest } from "./types";
import { ApiError } from "../api";

function readMemberId(value: string | undefined) {
    const memberId = Number(value);
    return Number.isInteger(memberId) && memberId > 0 ? memberId : null;
}

export function EditMemberPage() {
    const { memberId: memberIdParam } = useParams();
    const memberId = readMemberId(memberIdParam);
    const [formError, setFormError] = useState<string | null>(null);
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const memberQuery = useQuery({
        queryKey: ["member", "detail", memberId],
        queryFn: () => {
            if (memberId === null) {
                throw new Error("Invalid book id");
            }

            return getMember(memberId);
        },
        enabled: memberId !== null,
        retry: false,
    });

    const updateMutation = useMutation({
        mutationFn: (request: UpdateMemberRequest) => {
            if (memberId === null) {
                throw new Error("Invalid member id");
            }

            return updateMember(memberId, request);
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ["members"] });
            navigate(`/members/${memberId}`);
        },
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
            setFormError("Enter a name and valid email.");
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

    return (
        <main>
            <Link to={`/members/${memberId}`}>Cancel</Link>
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