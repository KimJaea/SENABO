package com.senabo.domain.report.entity;

import static com.querydsl.core.types.PathMetadataFactory.*;

import com.querydsl.core.types.dsl.*;

import com.querydsl.core.types.PathMetadata;
import javax.annotation.processing.Generated;
import com.querydsl.core.types.Path;
import com.querydsl.core.types.dsl.PathInits;


/**
 * QReport is a Querydsl query type for Report
 */
@Generated("com.querydsl.codegen.DefaultEntitySerializer")
public class QReport extends EntityPathBase<Report> {

    private static final long serialVersionUID = 114420640L;

    private static final PathInits INITS = PathInits.DIRECT2;

    public static final QReport report = new QReport("report");

    public final com.senabo.common.audit.QBaseEntity _super = new com.senabo.common.audit.QBaseEntity(this);

    public final NumberPath<Integer> communicationScore = createNumber("communicationScore", Integer.class);

    public final BooleanPath complete = createBoolean("complete");

    //inherited
    public final DateTimePath<java.time.LocalDateTime> createTime = _super.createTime;

    public final NumberPath<Integer> diseaseScore = createNumber("diseaseScore", Integer.class);

    public final NumberPath<Integer> endAffectionScore = createNumber("endAffectionScore", Integer.class);

    public final NumberPath<Integer> endStressScore = createNumber("endStressScore", Integer.class);

    public final NumberPath<Integer> feedScore = createNumber("feedScore", Integer.class);

    public final NumberPath<Long> id = createNumber("id", Long.class);

    public final com.senabo.domain.member.entity.QMember memberId;

    public final NumberPath<Integer> poopScore = createNumber("poopScore", Integer.class);

    public final NumberPath<Integer> startAffectionScore = createNumber("startAffectionScore", Integer.class);

    public final NumberPath<Integer> startStressScore = createNumber("startStressScore", Integer.class);

    public final NumberPath<Integer> totalTime = createNumber("totalTime", Integer.class);

    //inherited
    public final DateTimePath<java.time.LocalDateTime> updateTime = _super.updateTime;

    public final NumberPath<Integer> walkScore = createNumber("walkScore", Integer.class);

    public final NumberPath<Integer> week = createNumber("week", Integer.class);

    public QReport(String variable) {
        this(Report.class, forVariable(variable), INITS);
    }

    public QReport(Path<? extends Report> path) {
        this(path.getType(), path.getMetadata(), PathInits.getFor(path.getMetadata(), INITS));
    }

    public QReport(PathMetadata metadata) {
        this(metadata, PathInits.getFor(metadata, INITS));
    }

    public QReport(PathMetadata metadata, PathInits inits) {
        this(Report.class, metadata, inits);
    }

    public QReport(Class<? extends Report> type, PathMetadata metadata, PathInits inits) {
        super(type, metadata, inits);
        this.memberId = inits.isInitialized("memberId") ? new com.senabo.domain.member.entity.QMember(forProperty("memberId")) : null;
    }

}

