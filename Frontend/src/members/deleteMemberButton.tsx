import { useState } from "react";
import { useCurrentMember } from "../auth/useCurrentMember";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { deleteMember } from "./membersApi";
import { ApiError } from "../api";

type DeleteMemberButtonProps = {
    memberId: number,
    name: string
}

export function DeleteMemberButton({ memberId, name }: DeleteMemberButtonProps) {
    const [confirming, setConfirming] = useState(false);
    const currentMemberQuery = useCurrentMember();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    function leaveDeleteMember() {
        queryClient.invalidateQueries({
            queryKey: ["members"],
            refetchType: "none"
        });

        queryClient.removeQueries({
            queryKey: ["members", "details", memberId],
            exact: true
        });

        navigate("/members");
    }

    const deleteMutation = useMutation({
        mutationFn: () => deleteMember(memberId),
        onSuccess: leaveDeleteMember
    });

    if (!currentMemberQuery.isSuccess || currentMemberQuery.data.role !== "Administrator") {
        return null;
    }

    if (!confirming) {
        return (
            <button
                type="button"
                onClick={() => setConfirming(true)}>
                Delete member
            </button>
        );
    }

    const mutationStatus = deleteMutation.error instanceof ApiError ? deleteMutation.error.status : null;

    return (
        <section aria-labelledby="delete-member-heading">
            <h2 id="delete-member-heading">Delete {name}?</h2>
            <p>This action cannot be undone.</p>

            <button
                type="button"
                onClick={() => deleteMutation.mutate()}
                disabled={deleteMutation.isPending}
            >
                {deleteMutation.isPending ? "Deleting..." : "Yes, delete member"}
            </button>{" "}

            <button
                type="button"
                onClick={() => {
                    deleteMutation.reset();
                    setConfirming(false);
                }}>
                Cancel
            </button>

            {mutationStatus == 401 && <p>Your login is missing or expired.</p>}
            {mutationStatus == 403 && (
                <p>Only administrators can delete member.</p>
            )}
            {mutationStatus == 404 && (
                <div>
                    <p>This member no longer exists. It may already have been deleted.</p>
                    <button
                        type="button"
                        onClick={leaveDeleteMember}>
                        Back to members
                    </button>
                </div>
            )}
            {deleteMutation.isError && mutationStatus === null && (
                <p>Could not delete member.</p>
            )}
        </section>
    )

}