package com.senabo.domain.member.entity;

import static com.querydsl.core.types.PathMetadataFactory.*;

import com.querydsl.core.types.dsl.*;

import com.querydsl.core.types.PathMetadata;
import javax.annotation.processing.Generated;
import com.querydsl.core.types.Path;
import com.querydsl.core.types.dsl.PathInits;


/**
 * QMember is a Querydsl query type for Member
 */
@Generated("com.querydsl.codegen.DefaultEntitySerializer")
public class QMember extends EntityPathBase<Member> {

    private static final long serialVersionUID = -374739988L;

    public static final QMember member = new QMember("member1");

    public final com.senabo.common.audit.QBaseEntity _super = new com.senabo.common.audit.QBaseEntity(this);

    public final NumberPath<Integer> affection = createNumber("affection", Integer.class);

    public final ListPath<com.senabo.domain.affection.entity.Affection, com.senabo.domain.affection.entity.QAffection> affectionList = this.<com.senabo.domain.affection.entity.Affection, com.senabo.domain.affection.entity.QAffection>createList("affectionList", com.senabo.domain.affection.entity.Affection.class, com.senabo.domain.affection.entity.QAffection.class, PathInits.DIRECT2);

    public final ListPath<com.senabo.domain.bath.entity.Bath, com.senabo.domain.bath.entity.QBath> bathList = this.<com.senabo.domain.bath.entity.Bath, com.senabo.domain.bath.entity.QBath>createList("bathList", com.senabo.domain.bath.entity.Bath.class, com.senabo.domain.bath.entity.QBath.class, PathInits.DIRECT2);

    public final ListPath<com.senabo.domain.brushingTeeth.entity.BrushingTeeth, com.senabo.domain.brushingTeeth.entity.QBrushingTeeth> brushingTeethList = this.<com.senabo.domain.brushingTeeth.entity.BrushingTeeth, com.senabo.domain.brushingTeeth.entity.QBrushingTeeth>createList("brushingTeethList", com.senabo.domain.brushingTeeth.entity.BrushingTeeth.class, com.senabo.domain.brushingTeeth.entity.QBrushingTeeth.class, PathInits.DIRECT2);

    public final ListPath<com.senabo.domain.communication.entity.Communication, com.senabo.domain.communication.entity.QCommunication> communicationList = this.<com.senabo.domain.communication.entity.Communication, com.senabo.domain.communication.entity.QCommunication>createList("communicationList", com.senabo.domain.communication.entity.Communication.class, com.senabo.domain.communication.entity.QCommunication.class, PathInits.DIRECT2);

    public final BooleanPath complete = createBoolean("complete");

    //inherited
    public final DateTimePath<java.time.LocalDateTime> createTime = _super.createTime;

    public final StringPath deviceToken = createString("deviceToken");

    public final ListPath<com.senabo.domain.disease.entity.Disease, com.senabo.domain.disease.entity.QDisease> diseaseList = this.<com.senabo.domain.disease.entity.Disease, com.senabo.domain.disease.entity.QDisease>createList("diseaseList", com.senabo.domain.disease.entity.Disease.class, com.senabo.domain.disease.entity.QDisease.class, PathInits.DIRECT2);

    public final StringPath dogName = createString("dogName");

    public final StringPath email = createString("email");

    public final ListPath<com.senabo.domain.emergency.entity.Emergency, com.senabo.domain.emergency.entity.QEmergency> emergencyList = this.<com.senabo.domain.emergency.entity.Emergency, com.senabo.domain.emergency.entity.QEmergency>createList("emergencyList", com.senabo.domain.emergency.entity.Emergency.class, com.senabo.domain.emergency.entity.QEmergency.class, PathInits.DIRECT2);

    public final DateTimePath<java.time.LocalDateTime> enterTime = createDateTime("enterTime", java.time.LocalDateTime.class);

    public final DateTimePath<java.time.LocalDateTime> exitTime = createDateTime("exitTime", java.time.LocalDateTime.class);

    public final ListPath<com.senabo.domain.expense.entity.Expense, com.senabo.domain.expense.entity.QExpense> expenseList = this.<com.senabo.domain.expense.entity.Expense, com.senabo.domain.expense.entity.QExpense>createList("expenseList", com.senabo.domain.expense.entity.Expense.class, com.senabo.domain.expense.entity.QExpense.class, PathInits.DIRECT2);

    public final ListPath<com.senabo.domain.feed.entity.Feed, com.senabo.domain.feed.entity.QFeed> feedList = this.<com.senabo.domain.feed.entity.Feed, com.senabo.domain.feed.entity.QFeed>createList("feedList", com.senabo.domain.feed.entity.Feed.class, com.senabo.domain.feed.entity.QFeed.class, PathInits.DIRECT2);

    public final NumberPath<java.math.BigDecimal> houseLatitude = createNumber("houseLatitude", java.math.BigDecimal.class);

    public final NumberPath<java.math.BigDecimal> houseLongitude = createNumber("houseLongitude", java.math.BigDecimal.class);

    public final NumberPath<Long> id = createNumber("id", Long.class);

    public final ListPath<com.senabo.domain.report.entity.Report, com.senabo.domain.report.entity.QReport> reportList = this.<com.senabo.domain.report.entity.Report, com.senabo.domain.report.entity.QReport>createList("reportList", com.senabo.domain.report.entity.Report.class, com.senabo.domain.report.entity.QReport.class, PathInits.DIRECT2);

    public final ListPath<Role, EnumPath<Role>> roles = this.<Role, EnumPath<Role>>createList("roles", Role.class, EnumPath.class, PathInits.DIRECT2);

    public final EnumPath<Sex> sex = createEnum("sex", Sex.class);

    public final EnumPath<Species> species = createEnum("species", Species.class);

    public final NumberPath<Integer> stressLevel = createNumber("stressLevel", Integer.class);

    public final ListPath<com.senabo.domain.stress.entity.Stress, com.senabo.domain.stress.entity.QStress> stressList = this.<com.senabo.domain.stress.entity.Stress, com.senabo.domain.stress.entity.QStress>createList("stressList", com.senabo.domain.stress.entity.Stress.class, com.senabo.domain.stress.entity.QStress.class, PathInits.DIRECT2);

    public final NumberPath<Integer> totalTime = createNumber("totalTime", Integer.class);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> updateTime = _super.updateTime;

    public final ListPath<com.senabo.domain.walk.entity.Walk, com.senabo.domain.walk.entity.QWalk> walkList = this.<com.senabo.domain.walk.entity.Walk, com.senabo.domain.walk.entity.QWalk>createList("walkList", com.senabo.domain.walk.entity.Walk.class, com.senabo.domain.walk.entity.QWalk.class, PathInits.DIRECT2);

    public QMember(String variable) {
        super(Member.class, forVariable(variable));
    }

    public QMember(Path<? extends Member> path) {
        super(path.getType(), path.getMetadata());
    }

    public QMember(PathMetadata metadata) {
        super(Member.class, metadata);
    }

}

