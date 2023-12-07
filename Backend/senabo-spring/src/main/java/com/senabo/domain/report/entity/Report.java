package com.senabo.domain.report.entity;

import com.senabo.common.audit.BaseEntity;
import com.senabo.domain.member.entity.Member;
import jakarta.persistence.*;
import lombok.*;

@Getter
@Entity
@Builder
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@AllArgsConstructor
public class Report extends BaseEntity {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id", updatable = false, unique = true)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "member_id")
    private Member memberId;

    @Column(name = "week")
    private int week;

    @Column(name = "total_time")
    private int totalTime;

    @Column(name = "start_affection_score")
    private int startAffectionScore;

    @Column(name = "start_stress_score")
    private int startStressScore;

    @Column(name = "end_affection_score")
    private int endAffectionScore;

    @Column(name = "end_stress_score")
    private int endStressScore;

    @Column(name = "poop_score")
    private int poopScore;

    @Column(name = "walk_score")
    private int walkScore;

    @Column(name = "feed_score")
    private int feedScore;

    @Column(name = "communication_score")
    private int communicationScore;

    @Column(name = "disease_score")
    private int diseaseScore;

    @Column(name = "complete")
    private Boolean complete;

    public Report(Member memberId, int week, int startAffectionScore, int startStressScore) {
        this.memberId = memberId;
        this.week = week;
        this.startAffectionScore = startAffectionScore;
        this.startStressScore = startStressScore;
        this.totalTime = 0;
        this.endAffectionScore = 0;
        this.endStressScore = 0;
        this.poopScore = 0;
        this.walkScore = 0;
        this.feedScore = 0;
        this.communicationScore = 0;
        this.diseaseScore = 0;
        this.complete = false;
    }

    public void updateTotalTime(int totalTime){
        this.totalTime += totalTime;
    }

    public void update(int endAffectionScore, int endStressScore, int poopScore, int walkScore, int feedScore, int communicationScore, int diseaseScore) {
        this.endAffectionScore = endAffectionScore;
        this.endStressScore = endStressScore;
        this.poopScore = poopScore;
        this.walkScore = walkScore;
        this.feedScore = feedScore;
        this.communicationScore = communicationScore;
        this.diseaseScore = diseaseScore;
        this.complete = true;
    }

}
