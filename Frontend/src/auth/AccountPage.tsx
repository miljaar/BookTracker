import { Link, Navigate } from "react-router-dom";
import { ApiError } from "../api";
import { useCurrentMember } from "./useCurrentMember";
import { getAccessToken } from "./tokenStorage";

export function AccountPage() {
    const currentMemberQuery = useCurrentMember();


    if (!getAccessToken())
        return <Navigate to="/login" replace />;

    if (currentMemberQuery.isPending) {
        return <p>Loading account...</p>;
    }

    const unauthorized =
        currentMemberQuery.error instanceof ApiError &&
        currentMemberQuery.error.status === 401;

    if (unauthorized)
        return <Navigate to="/login" replace />;

    if (currentMemberQuery.isError) {
        return <p>Could not lod the account.</p>
    }

    const member = currentMemberQuery.data;

    return (
        <main>
            <h1>{member.name}</h1>
            <p>{member.email}</p>
            <p>Role: {member.role}</p>
            <Link to="/account/edit">Edit account</Link>
        </main>
    );
}