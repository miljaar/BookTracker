import { useQuery } from "@tanstack/react-query";
import { Link, useParams } from "react-router-dom";
import { getMember } from "./membersApi";
import { ApiError } from "../api";
import { EditMemberLink } from "./editMemberLink";
import { DeleteMemberButton } from "./deleteMemberButton";

function readMemberId(value: string | undefined) {
    const memberId = Number(value);
    return Number.isInteger(memberId) && memberId > 0 ? memberId : null;
}

export function MemberDetailsPage() {
    const { memberId: memberIdParameter } = useParams();
    const memberId = readMemberId(memberIdParameter)

    if (memberId === null) {
        return (
            <main>
                <p>Invalid member id.</p>
                <Link to="/members">Back to members</Link>
            </main>
        );
    }

    const memberQuery = useQuery({
        queryKey: ["member", "detail", memberId],
        queryFn: () => getMember(memberId)
    });

    if (memberQuery.isPending) {
        return <p>Loading member...</p>;
    }

    if (memberQuery.error instanceof ApiError && memberQuery.error.status === 404) {
        return (
            <main>
                <p>Member not found</p>
                <Link to="/members">Back to members</Link>
            </main>);
    }

    if (memberQuery.isError) {
        return (
            <main>
                <h1>Could not load member</h1>
                <p>Is the API running?</p>
                <Link to="/members">Back to members</Link>
            </main>
        );
    }

    const member = memberQuery.data;
    return (
        <main>
            <Link to="/members">Back to members</Link>
            <h1>{member.name}</h1>
            <p>{member.email}</p>
            <EditMemberLink memberId={member.id} />
            <DeleteMemberButton memberId={member.id} name={member.name} />
        </main>
    );
}

