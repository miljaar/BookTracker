import { Link } from "react-router-dom";
import { useCurrentMember } from "../auth/useCurrentMember"

type EditMemberLinkProp = {
    memberId: number
}

export function EditMemberLink({ memberId }: EditMemberLinkProp) {
    const currentMemberQuery = useCurrentMember();

    if (!currentMemberQuery.isSuccess || currentMemberQuery.data.role !== "Administrator") {
        return null;
    }

    return <Link to={`/members/${memberId}/edit`}>Edit member</Link>
}