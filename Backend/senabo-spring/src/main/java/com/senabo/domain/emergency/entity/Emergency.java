package com.senabo.domain.emergency.entity;

import com.senabo.common.audit.BaseEntity;
import com.senabo.domain.member.entity.Member;
import jakarta.persistence.*;
import lombok.*;

@Getter
@Entity
@Builder
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@AllArgsConstructor
public class Emergency extends BaseEntity {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id", updatable = false, unique = true)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "member_id")
    private Member memberId;

    @Column(name = "type")
    private EmergencyType type;

    @Column(name = "solved")
    private boolean solved;

    public Emergency (Member member, EmergencyType type, boolean solved){
        this.memberId = member;
        this.type = type;
        this.solved = solved;
    }

    public void update (boolean solved){
        this.solved = solved;
    }
}
