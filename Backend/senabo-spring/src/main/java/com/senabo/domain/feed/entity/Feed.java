package com.senabo.domain.feed.entity;

import com.senabo.common.audit.BaseEntity;
import com.senabo.domain.member.entity.Member;
import jakarta.persistence.*;
import lombok.*;

@Getter
@Entity
@Builder
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@AllArgsConstructor
public class Feed extends BaseEntity {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id", updatable = false, unique = true)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "member_id")
    private Member memberId;

    @Column(name = "clean_yn")
    private Boolean cleanYn;

    public Feed(Member memberId) {
        this.memberId = memberId;
        this.cleanYn = false;
    }

    public void update(){
        this.cleanYn = true;
    }

    public void setMemberId(Member memberId) {
        this.memberId = memberId;
    }
}
