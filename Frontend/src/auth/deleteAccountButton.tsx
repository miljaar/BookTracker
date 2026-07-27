import { useState } from "react";
import { useCurrentMember } from "./useCurrentMember";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { deleteMember } from "../members/membersApi";
import { ApiError } from "../api";
import { removeAccessToken } from "./tokenStorage";

type DeleteAccountButtonProps = {
    memberId: number,
    memberName: string
}

export function DeleteAccountButton({ memberId, memberName }: DeleteAccountButtonProps) {
    const [confirming, setConfirming] = useState(false);
    const currentMemberQuery = useCurrentMember();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    function leaveDeletedAccount() {
        queryClient.invalidateQueries({
            queryKey: ["members"],
            refetchType: "none"
        });

        queryClient.removeQueries({
            queryKey: ["members", "detail", memberId],
            exact: true
        });
        removeAccessToken();
        queryClient.removeQueries({ queryKey: ["current-member"] });
        navigate("/");
    }

    const deleteMutation = useMutation({
        mutationFn: () => deleteMember(memberId),
        onSuccess: leaveDeletedAccount
    });

    if (!currentMemberQuery.isSuccess || currentMemberQuery.data.id !== memberId) {
        return null;
    }

    if (!confirming) {
        return (
            <button type="button" onClick={() => setConfirming(true)}>
                Delete account
            </button>
        );
    }

    const mutationStatus = deleteMutation.error instanceof ApiError ? deleteMutation.error.status : null;


    return (
        <section aria-labelledby="delete-account-heading">
            <h2 id="delete-account-heading">Delete {memberName}?</h2>
            <p>This action cannot be undone.</p>

            <button
                type="button"
                onClick={() => deleteMutation.mutate()}
                disabled={deleteMutation.isPending}
            >
                {deleteMutation.isPending ? "Deleting..." : "Yes, delete account"}
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
                <p>Only administrators can delete account.</p>
            )}
            {mutationStatus == 404 && (
                <div>
                    <p>This account no longer exists. It may already have been deleted.</p>
                    <button
                        type="button"
                        onClick={leaveDeletedAccount}>
                        Back to members
                    </button>
                </div>
            )}
            {deleteMutation.isError && mutationStatus === null && (
                <p>Could not delete account.</p>
            )}
        </section>
    )
}